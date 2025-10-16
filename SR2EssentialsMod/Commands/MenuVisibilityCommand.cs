using System;
using SR2E.Managers;
using SR2E.Storage;

namespace SR2E.Commands;

internal class MenuVisibilityCommands
{
    internal class OpenCommand : SR2ECommand
    {
        internal OpenCommand(MenuIdentifier identifier, SR2EMenu menu, bool inGameOnly)
        {
            this.identifier = identifier;
            this.menu = menu;
            this.inGameOnly = inGameOnly;
        }
        MenuIdentifier identifier = new MenuIdentifier();
        SR2EMenu menu;
        private bool inGameOnly;
        public override string ID => "open"+identifier.saveKey.ToLower();
        public override string Usage => "open"+identifier.saveKey.ToLower();
        public override Type[] execWhileMenuOpen => new[] { menu.GetType() };
        public override CommandType type => CommandType.DontLoad | CommandType.Menu;
        public override string Description => translation($"cmd.openmenu.description");
        public override string ExtendedDescription
        {
            get
            {
                string key = $"cmd.openmenu.extendeddescription";
                string translation = SR2ELanguageManger.translation(key);
                return key == translation ? Description : translation;
            }
        }
        public override bool Execute(string[] args)
        {
            if (!args.IsBetween(0,0)) return SendNoArguments();
            if (inGameOnly) if(!inGame) return SendLoadASaveFirst();
            if (menu.isOpen) return false;
            MenuEUtil.CloseOpenMenu();
            menu.Open();
            return true;
        }
    }
    internal class CloseCommand : SR2ECommand
    {
        internal CloseCommand(MenuIdentifier identifier, SR2EMenu menu, bool inGameOnly)
        {
            this.identifier = identifier;
            this.menu = menu;
            this.inGameOnly = inGameOnly;
        }
        MenuIdentifier identifier = new MenuIdentifier();
        SR2EMenu menu;
        private bool inGameOnly;
        public override string ID => "close"+identifier.saveKey.ToLower();
        public override string Usage => "close"+identifier.saveKey.ToLower();
        public override Type[] execWhileMenuOpen => new[] { menu.GetType() };
        public override CommandType type => CommandType.DontLoad | CommandType.Menu;
        public override string Description => translation($"cmd.closemenu.description");
        public override string ExtendedDescription
        {
            get
            {
                string key = $"cmd.closemenu.extendeddescription";
                string translation = SR2ELanguageManger.translation(key);
                return key == translation ? Description : translation;
            }
        }
        public override bool Execute(string[] args)
        {
            if (!args.IsBetween(0,0)) return SendNoArguments();
            if (inGameOnly) if(!inGame) return SendLoadASaveFirst();
            if (!menu.isOpen) return false;
            menu.Close();
            return true;
        }
    }
    internal class ToggleCommand : SR2ECommand
    {
        internal ToggleCommand(MenuIdentifier identifier, SR2EMenu menu, bool inGameOnly)
        {
            this.identifier = identifier;
            this.menu = menu;
            this.inGameOnly = inGameOnly;
        }
        MenuIdentifier identifier = new MenuIdentifier();
        SR2EMenu menu;
        private bool inGameOnly;
        public override string ID => "toggle"+identifier.saveKey.ToLower();
        public override string Usage => "toggle"+identifier.saveKey.ToLower();
        public override Type[] execWhileMenuOpen => new[] { menu.GetType() };
        public override CommandType type => CommandType.DontLoad | CommandType.Menu;
        public override string Description => translation($"cmd.togglemenu.description");
        public override string ExtendedDescription
        {
            get
            {
                string key = $"cmd.togglemenu.extendeddescription";
                string translation = SR2ELanguageManger.translation(key);
                return key == translation ? Description : translation;
            }
        }
        public override bool Execute(string[] args)
        {
            if (!args.IsBetween(0,0)) return SendNoArguments();
            if (inGameOnly) if(!inGame) return SendLoadASaveFirst();
            if (!menu.isOpen) MenuEUtil.CloseOpenMenu();
            menu.Toggle();
            return true;
        }
    }
}