using System.Collections.Generic;

namespace Invector.vItemManager
{
    using vCharacterController;
    [vClassHeader("Check if can Add Health", "Simple Example to verify if the health item can be used based on the character's health is full or not.", openClose = false)]
    public class vCheckCanAddHealth : vMonoBehaviour
    {
        public vItemManager itemManager;        
        public vThirdPersonController tpController;
        public bool getInParent = true;
        internal bool canUse;
        internal bool firstRun;

        private void Start()
        {            
            if(itemManager == null)
            {
                if(getInParent)
                    itemManager = GetComponentInParent<vItemManager>();
                else
                    itemManager = GetComponent<vItemManager>();
            }

            if (tpController == null)
            {
                if (getInParent)
                    tpController = GetComponentInParent<vThirdPersonController>();
                else
                    tpController = GetComponent<vThirdPersonController>();
            }            

            if (itemManager)
            {
                itemManager.canUseItemDelegate -= new vItemManager.CanUseItemDelegate(CanUseItem);
                itemManager.canUseItemDelegate += new vItemManager.CanUseItemDelegate(CanUseItem);
            }
        }

        private void OnDestroy()
        {
            var itemManager = GetComponent<vItemManager>();
            if (itemManager)
                itemManager.canUseItemDelegate -= new vItemManager.CanUseItemDelegate(CanUseItem);
        }

        private void CanUseItem(vItem item, ref List<bool> validateResult)
        {
            if (item.GetItemAttribute(vItemAttributes.Health) != null)
            {
                var valid = tpController.currentHealth < tpController.maxHealth;
                if(valid != canUse || !firstRun)
                {
                    canUse = valid;
                    firstRun = true;
                    vHUDController.instance.ShowText(valid ? "Increase health" : "Can't use " + item.name + " because your health is full", 4f);
                }
                
                if (!valid)
                    validateResult.Add(valid);
            }
        }
    }
}