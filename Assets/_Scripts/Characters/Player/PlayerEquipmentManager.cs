using Globals;
using Items;
using UnityEngine;

namespace Character.Player
{
    public class PlayerEquipmentManager : CharacterEquipmentManager
    {
        protected PlayerManager _playerManager;

        public InstantiationSlot rightHandSlot;
        public InstantiationSlot leftHandSlot;

        [SerializeField] WeaponManager _rightWeaponManager;
        [SerializeField] WeaponManager _leftWeaponManager;

        public GameObject rightHandItemModel;
        public GameObject leftHandItemModel;

        private int currentRightHandItemId;
        public int CurrentRightHandItemId
        {
            get => currentRightHandItemId;
            set
            {
                if (currentRightHandItemId != value) 
                {
                    //OnRightHandItemIdChange?.Invoke(currentRightHandItemId, value);
                    currentRightHandItemId = value;

                    WeaponItem newWeapon = WorldItemDatabase.instance.GetWeaponById(value);
                    _playerManager._inventoryManager.currentRightHandItem = newWeapon;
                    LoadRightItem();

                }
            }
        }
        //public event Action<int, int> OnRightHandItemIdChange;

        protected override void Awake()
        {
            base.Awake();

            _playerManager = GetComponent<PlayerManager>();

            InitializeSlots();
        }

        protected override void Start()
        {
            base.Start();

            LoadItemOnBothHands();
        }

        private void InitializeSlots()
        {
            InstantiationSlot[] slots = GetComponentsInChildren<InstantiationSlot>();

            for (int i = 0; i < slots.Length; ++i) 
            {
                switch(slots[i].slot) 
                {
                    case ItemModelSlot.RightHand:
                        rightHandSlot = slots[i];
                        break;
                    case ItemModelSlot.LeftHand:
                        leftHandSlot = slots[i];
                        break;
                }
            }
        }
    
        public void LoadItemOnBothHands()
        {
            LoadRightItem();
            LoadLeftItem();
        }

        public void LoadRightItem()
        {
            if (_playerManager._inventoryManager.currentRightHandItem != null)
            {
                // Remove old weapon
                rightHandSlot.UnloadModel(); 

                //Bring new weapon
                rightHandItemModel = Instantiate(_playerManager._inventoryManager.currentRightHandItem.model);
                rightHandSlot.LoadModel(rightHandItemModel);

                _rightWeaponManager = rightHandItemModel.GetComponent<WeaponManager>();
                if(_rightWeaponManager != null ) 
                {
                    _rightWeaponManager.SetWeaponDamage(_playerManager, (WeaponItem)_playerManager._inventoryManager.currentRightHandItem);
                }
                // else is a different type of displayable item
            }
        }

        public void LoadLeftItem() 
        {
            if (_playerManager._inventoryManager.currentLeftHandItem != null)
            {
                leftHandSlot.UnloadModel();

                //Bring new weapon
                leftHandItemModel = Instantiate(_playerManager._inventoryManager.currentLeftHandItem.model);
                leftHandSlot.LoadModel(leftHandItemModel);

                _leftWeaponManager = leftHandItemModel.GetComponent<WeaponManager>();
                if (_leftWeaponManager != null )
                {
                    _leftWeaponManager.SetWeaponDamage(_playerManager, (WeaponItem)_playerManager._inventoryManager.currentLeftHandItem);
                }
            }
        }

        public void SwitchRightItem()
        {
            _playerManager._playerAnimatorManager.PlayTargetActionAnimation("Equip", false, true, true, true);

            WeaponItem selectedWeapon = null;

            _playerManager._inventoryManager.rightHandWeaponIndex += 1;
            // Reset if index out of bounds
            if (_playerManager._inventoryManager.rightHandWeaponIndex < 0 || _playerManager._inventoryManager.rightHandWeaponIndex > 2)
            {
                _playerManager._inventoryManager.rightHandWeaponIndex = 0;

                // Check if we are holding more than one weapon
                float weaponCount = 0;
                WeaponItem firstWeapon = null;
                int firstWeaponPosition = 0;

                for (int i = 0; i < _playerManager._inventoryManager.weaponsInRightHandSlots.Length; ++i)
                {
                    if (_playerManager._inventoryManager.weaponsInRightHandSlots[i].itemId != WorldItemDatabase.instance.unarmedWeapon.itemId)
                    {
                        weaponCount += 1;

                        if (firstWeapon == null)
                        {
                            firstWeapon = _playerManager._inventoryManager.weaponsInRightHandSlots[i];
                            firstWeaponPosition = i;
                        }
                    }
                }

                if (weaponCount <= 1)
                {
                    _playerManager._inventoryManager.rightHandWeaponIndex = -1;
                    selectedWeapon = WorldItemDatabase.instance.unarmedWeapon;
                    CurrentRightHandItemId = selectedWeapon.itemId;
                }
                else
                {
                    _playerManager._inventoryManager.rightHandWeaponIndex = firstWeaponPosition;
                    CurrentRightHandItemId = firstWeapon.itemId;
                }

                return;
            }

            foreach (WeaponItem weapon in _playerManager._inventoryManager.weaponsInRightHandSlots)
            {
                // If this weapon is not Unarmed weapon
                if (_playerManager._inventoryManager.weaponsInRightHandSlots[_playerManager._inventoryManager.rightHandWeaponIndex] != WorldItemDatabase.instance.unarmedWeapon)
                {
                    selectedWeapon = _playerManager._inventoryManager.weaponsInRightHandSlots[_playerManager._inventoryManager.rightHandWeaponIndex];
                    CurrentRightHandItemId = selectedWeapon.itemId;
                    return;
                }
            }

            if (selectedWeapon == null && _playerManager._inventoryManager.rightHandWeaponIndex <= 2)
            {
                SwitchRightItem();
            }
        }
    }
}