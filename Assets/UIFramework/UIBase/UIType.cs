using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace MoleMole
{
	public class UIType {

        public string Path { get; private set; }

        public string Name { get; private set; }

        public UIType(string path)
        {
            Path = path;
            Name = path.Substring(path.LastIndexOf('/') + 1);
        }

        public override string ToString()
        {
            return string.Format("path : {0} name : {1}", Path, Name);
        }

        public static readonly UIType StartView = new UIType("View/StartView");
        public static readonly UIType LevelView = new UIType("View/LevelView");
        public static readonly UIType GameView = new UIType("View/GameView");
        public static readonly UIType GameOverView = new UIType("View/GameOverView");
        public static readonly UIType GameRestartView = new UIType("View/GameRestartView");
        public static readonly UIType ScoreWinView = new UIType("View/ScoreWinView");
        public static readonly UIType ScoreFailedView = new UIType("View/ScoreFailedView");
        public static readonly UIType StarWinView = new UIType("View/StarWinView");
        public static readonly UIType StarFailedView = new UIType("View/StarFailedView");
    }
}
