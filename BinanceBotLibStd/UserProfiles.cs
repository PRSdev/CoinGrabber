using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace BinanceBotLib
{
    public class UserProfiles : SettingsBase<UserProfiles>
    {
        public ObservableCollection<UserData> Users { get; set; } = new ObservableCollection<UserData>();
    }
}