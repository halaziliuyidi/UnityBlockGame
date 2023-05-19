
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
    public class StarFailedViewContext : BaseContext
    {
        public StarFailedViewContext() : base(UIType.StarFailedView)
        {

        }
    }
    public class StarFailedView : BaseView
    {
        public GameObject starObject;

        public Transform levelModeStarObject;

        public GameObject[] stars;

        public override void OnEnter(BaseContext context)
        {
            SpawnStars();
        }

        public override void OnExit(BaseContext context)
        {
            base.OnExit(context);
        }

        public override void OnPause(BaseContext context)
        {
            base.OnPause(context);
        }

        public override void OnResume(BaseContext context)
        {
            base.OnResume(context);
        }

        private void SpawnStars()
        {
            List<int> starKinds = GameManager.Instance.nowLevel.GetStarKinds();
            Debug.Log(starKinds.Count);
            if (stars != null)
            {
                for (int i = 0; i < stars.Length; i++)
                {
                    if (stars[i] != null)
                    {
                        Destroy(stars[i]);
                    }
                }
            }
            stars = new GameObject[5];
            for (int i = 0; i < starKinds.Count; i++)
            {
                GameObject star = Instantiate(starObject, levelModeStarObject.transform);
                Image starImage = star.GetComponent<Image>();
                starImage.sprite = GameConstManager.Instance.StarSprites[starKinds[i]];
                stars[starKinds[i]] = star;
                int starNum = GameManager.Instance.nowLevel.startTargetNum[starKinds[i]];
                TextMeshProUGUI numtext = star.transform.Find("num").GetComponent<TextMeshProUGUI>();
                GameObject gou = star.transform.Find("gou").gameObject;
                if (starNum <= 0)
                {
                    gou.SetActive(true);
                    numtext.gameObject.SetActive(false);
                }
                else
                {
                    numtext.text = starNum.ToString();
                    gou.SetActive(false);
                    numtext.gameObject.SetActive(true);
                }
                star.SetActive(true);
            }
        }

        public void OnRetryBtnClick()
        {
            GameManager.Instance.RetryLevel();
            Singleton<ContextManager>.Instance.Push(new GameViewContext());
        }
    }
}
