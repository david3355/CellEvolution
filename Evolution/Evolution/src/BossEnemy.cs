using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Evolution
{
    class BossEnemy : IntelligentEnemy
    {
        public void ChangeVelocity(System.Collections.Generic.List<Cell> Objects, Player Player, float speed, int level)
        {
            this.ChangeVelocity(Objects, Player, speed + 0.2f + (level / 6 - 1) * 0.1f);
        }
    }
}
