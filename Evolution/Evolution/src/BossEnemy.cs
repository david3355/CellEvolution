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
        public override void ChangeVelocity(System.Collections.Generic.List<Cell> Objects, Player Player, float speed)
        {
            base.ChangeVelocity(Objects, Player, speed + 0.9f);
        }
    }
}
