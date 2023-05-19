using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Text;
using UnityEngine.UI;
using System.Threading.Tasks;
using System;
using System.Reflection;

namespace MoleMole
{
    public class GameViewContext : BaseContext
    {
        public GameViewContext() : base(UIType.GameView)
        {

        }
    }

    public class GameView : BaseView
    {
        public GameObject scoreUIObject;

        public GameObject levelModeStarObject;

        public GameObject levelModeNoStarObject;

        public GameObject noLevelModeObject;

        public GameObject starObject;

        public TextMeshProUGUI scoreText;

        public TextMeshProUGUI scoreProgressText;

        public TextMeshProUGUI comboText;

        public TextMeshProUGUI addScoreText;

        public TextMeshProUGUI maxScoreText;

        public Button cancelBtn;

        public Image progressImage;

        public ScoreObjectAnimator addScoreObject;

        public ScoreObjectAnimator comboObject;

        public ScoreObjectAnimator goodObject;

        public ScoreObjectAnimator greatObject;

        public ScoreObjectAnimator niceObject;

        public ScoreObjectAnimator amazingObject;

        public ScoreObjectAnimator newHightScoreObject;

        public GameObject noSpaceTip;

        public GameObject settingView;

        public Button settingBtn;

        private Coroutine addScoreCor;

        private StringBuilder scoreString = new StringBuilder();

        private int targetScore;

        [SerializeField]
        private TextMeshProUGUI[] starNumTexts;

        private int changeIndex;

        public override void OnEnter(BaseContext context)
        {
            if (GameDataManager.Instance.firstGame)
            {
                settingBtn.gameObject.SetActive(false);
            }
            settingView.SetActive(false);
            maxScoreText.text = GameDataManager.Instance.GetMaxScore().ToString();
            addScoreObject.gameObject.SetActive(false);
            comboObject.gameObject.SetActive(false);
            goodObject.gameObject.SetActive(false);
            greatObject.gameObject.SetActive(false);
            niceObject.gameObject.SetActive(false);
            amazingObject.gameObject.SetActive(false);
            newHightScoreObject.gameObject.SetActive(false);
            noSpaceTip.SetActive(false);
            scoreString.Clear();
            scoreText.text = GameManager.Instance.nowGameScore.ToString();

            EventManager.AddListener(GameConstManager.addBaseScore, OnAddBaseScore);
            EventManager.AddListener(GameConstManager.addScoreEvent, OnAddScore);
            EventManager.AddListener(GameConstManager.maxScoreEvent, OnMaxScore);
            EventManager.AddListener(GameConstManager.comboEvent, OnCombo);
            EventManager.AddListener(GameConstManager.maxComboEvent, OnMaxCombo);
            EventManager.AddListener(GameConstManager.gameOver, OnGameOver);
            EventManager.AddListener(GameConstManager.teachEnd, OnTeachEnd);
            EventManager.AddListener(GameConstManager.gameScoreWin, OnScoreWin);
            EventManager.AddListener(GameConstManager.gameStarWin, OnStartWin);
            EventManager.AddListener(GameConstManager.gameScoreFailed, OnScoreFailed);
            EventManager.AddListener(GameConstManager.gameStarFailed, OnStarFailed);
            EventManager.AddListener(GameConstManager.SubtractStar, OnSubtractStar);
            EventManager.AddListener(GameConstManager.SubtractStarUpdateUI, OnUpdateSubtractStarUI);

            if (GameManager.Instance.nowIsLevelMode)
            {
                maxScoreText.transform.parent.gameObject.SetActive(false);
                cancelBtn.gameObject.SetActive(true);
                if (GameManager.Instance.nowLevel.isStarLevel)
                {
                    levelModeStarObject.gameObject.SetActive(true);
                    scoreUIObject.gameObject.SetActive(false);
                    SpawnStarts();
                }
                else
                {
                    levelModeStarObject.gameObject.SetActive(false);
                    noLevelModeObject.gameObject.SetActive(false);
                    levelModeNoStarObject.gameObject.SetActive(true);
                    scoreUIObject.gameObject.SetActive(true);
                    progressImage.fillAmount = GameManager.Instance.nowGameScore / GameManager.Instance.nowLevel.targetScore;
                    scoreString.Clear();
                    scoreString.Append(GameManager.Instance.nowGameScore);
                    scoreString.Append("/");
                    scoreString.Append(GameManager.Instance.nowLevel.targetScore);
                    scoreProgressText.text = scoreString.ToString();
                    scoreString.Clear();
                }
            }
            else
            {
                maxScoreText.transform.parent.gameObject.SetActive(true);
                cancelBtn.gameObject.SetActive(false);
                levelModeStarObject.gameObject.SetActive(false);
                levelModeNoStarObject.gameObject.SetActive(false);
                noLevelModeObject.gameObject.SetActive(true);
                scoreUIObject.gameObject.SetActive(true);
            }
            base.OnEnter(context);
        }

        public override void OnExit(BaseContext context)
        {
            scoreString.Clear();
            EventManager.RemoveListener(GameConstManager.addBaseScore, OnAddBaseScore);
            EventManager.RemoveListener(GameConstManager.addScoreEvent, OnAddScore);
            EventManager.RemoveListener(GameConstManager.maxScoreEvent, OnMaxScore);
            EventManager.RemoveListener(GameConstManager.comboEvent, OnCombo);
            EventManager.RemoveListener(GameConstManager.maxComboEvent, OnMaxCombo);
            EventManager.RemoveListener(GameConstManager.gameOver, OnGameOver);
            EventManager.RemoveListener(GameConstManager.teachEnd, OnTeachEnd);
            EventManager.RemoveListener(GameConstManager.gameScoreWin, OnScoreWin);
            EventManager.RemoveListener(GameConstManager.gameStarWin, OnStartWin);
            EventManager.RemoveListener(GameConstManager.gameScoreFailed, OnScoreFailed);
            EventManager.RemoveListener(GameConstManager.gameStarFailed, OnStarFailed);
            EventManager.RemoveListener(GameConstManager.SubtractStar, OnSubtractStar);
            EventManager.RemoveListener(GameConstManager.SubtractStarUpdateUI, OnUpdateSubtractStarUI);
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

        public void OnAddBaseScore(object[] arg)
        {
            DebugHelper.LogFormat("Add Base Score: {0}", arg[0]);
            StartAddScore((int)arg[1], (int)arg[0], 0.3f);
            targetScore = (int)arg[0];
        }

        public void OnAddScore(object[] arg)
        {
            DebugHelper.LogFormat("Add Score: {0}", arg[0]);
            StartAddScore((int)arg[1], (int)arg[0], 0.5f);
            targetScore = (int)arg[0];
            int line = (int)arg[2];
            switch (line)
            {
                case 2:
                    goodObject.PlayAnimator(Vector3.one);
                    goodObject.transform.position = (Vector3)arg[3] + new Vector3(0, 1);
                    break;
                case 3:
                    niceObject.PlayAnimator(Vector3.one);
                    niceObject.transform.position = (Vector3)arg[3] + new Vector3(0, 1);
                    break;
                case 4:
                    greatObject.PlayAnimator(Vector3.one);
                    greatObject.transform.position = (Vector3)arg[3] + new Vector3(0, 1);
                    break;
                default:
                    if (line >= 5)
                    {
                        amazingObject.PlayAnimator(Vector3.one);
                        amazingObject.transform.position = (Vector3)arg[3] + new Vector3(0, 1);
                    }
                    break;
            }
            addScoreText.text = Tool.GetText((int)arg[0] - (int)arg[1]);
            addScoreObject.transform.position = (Vector3)arg[3];
            addScoreObject.PlayAnimator(Vector3.one);
        }

        public void OnMaxScore(object[] arg)
        {
            if (GameManager.Instance.nowIsLevelMode)
            {
                return;
            }
            maxScoreText.text = arg[0].ToString();
            if (!(bool)arg[1] && (int)arg[0] > 280)
            {
                newHightScoreObject.PlayAnimator(Vector3.one);
                GameManager.Instance.ShowedHighScore();
            }
            DebugHelper.LogFormat("Max Score: {0}", arg[0]);
        }

        public void OnCombo(object[] arg)
        {
            comboText.text = Tool.GetText((int)arg[0]);
            Vector3 vector3 = (Vector3)arg[1];
            if (vector3.x >= 3.5)
            {
                comboObject.transform.position = vector3 + new Vector3(-2f, 0, 0);
            }
            else
            {
                comboObject.transform.position = vector3 + new Vector3(2f, 0, 0);
            }

            comboObject.PlayAnimator(Vector3.one * 2f, 0.3f);
            DebugHelper.LogFormat("Combo: {0}", arg[0]);
        }

        public void OnMaxCombo(object[] arg)
        {
            DebugHelper.LogFormat("Max Combo: {0}", arg[0]);
        }

        private Coroutine showNoSpaceCor;

        public void OnGameOver(object[] arg)
        {
            DebugHelper.Log("Game Over");
            StopNoSpaceCor();
            showNoSpaceCor = StartCoroutine(ShowNoSpace());
        }

        private void StopNoSpaceCor()
        {
            if (showNoSpaceCor != null)
            {
                StopCoroutine(showNoSpaceCor);
            }
            showNoSpaceCor = null;
        }

        private IEnumerator ShowNoSpace()
        {
            noSpaceTip.SetActive(true);
            float showTime = 3;
            while (showTime > 0)
            {
                showTime -= Time.deltaTime;
                yield return null;
            }
            GameManager.Instance.audioController.PlayFailedAudio();
            if (GameManager.Instance.nowIsLevelMode)
            {
                if (GameManager.Instance.nowLevel.isStarLevel)
                {
                    Singleton<ContextManager>.Instance.Push(new StarFailedViewContext());
                }
                else
                {
                    Singleton<ContextManager>.Instance.Push(new ScoreFailedViewContext());
                }
            }
            else
            {
                Singleton<ContextManager>.Instance.Push(new GameOverViewContext());
            }
        }

        private void StartAddScore(int startValue, int targetValue, float duration)
        {
            if (GameManager.Instance.gameOver || GameManager.Instance.paused)
            {
                return;
            }
            StopAddScore();
            addScoreCor = null;
            addScoreCor = StartCoroutine(Count(startValue, targetValue, duration));
        }

        private void StopAddScore()
        {
            if (addScoreCor != null)
            {
                StopCoroutine(addScoreCor);
            }
            addScoreCor = null;
            scoreString.Clear();
            scoreString.Append(targetScore);
            scoreText.text = scoreString.ToString();
            scoreString.Length = 0;
        }

        private IEnumerator Count(int startValue, int targetValue, float duration)
        {
            TextMeshProUGUI textMeshProUGUI = null;
            bool isProgress;
            if (GameManager.Instance.nowIsLevelMode && !GameManager.Instance.nowLevel.isStarLevel)
            {
                textMeshProUGUI = scoreProgressText;
                isProgress = true;
            }
            else
            {
                textMeshProUGUI = scoreText;
                isProgress = false;
            }
            float time = 0f;
            while (time < duration)
            {
                scoreString.Clear();
                time += Time.deltaTime;
                float value = Mathf.Lerp(startValue, targetValue, time / duration);
                scoreString.Append(Mathf.RoundToInt(value).ToString());
                if (isProgress)
                {
                    scoreString.Append("/");
                    scoreString.Append(GameManager.Instance.nowLevel.targetScore);
                    progressImage.fillAmount = value / GameManager.Instance.nowLevel.targetScore;
                }
                textMeshProUGUI.text = scoreString.ToString();
                yield return null;
            }
            scoreString.Clear();
            scoreString.Append(targetValue);
            if (isProgress)
            {
                scoreString.Append("/");
                scoreString.Append(GameManager.Instance.nowLevel.targetScore);
            }
            textMeshProUGUI.text = scoreString.ToString();
            scoreString.Length = 0;
        }

        public void OnSettingButongClick()
        {
            GameManager.Instance.audioController.PlayButtonClickAudio();
            settingView.gameObject.SetActive(true);
        }

        public void OnTeachEnd(object[] args)
        {
            settingBtn.gameObject.SetActive(true);
        }


        private void SpawnStarts()
        {
            List<int> starKinds = GameManager.Instance.nowLevel.GetStarKinds();
            Debug.Log(starKinds.Count);
            if (starNumTexts != null)
            {
                for (int i = 0; i < starNumTexts.Length; i++)
                {
                    if (starNumTexts[i] != null)
                    {
                        Destroy(starNumTexts[i].transform.parent.gameObject);
                    }
                }
            }
            starNumTexts = new TextMeshProUGUI[5];
            Vector3[] starPos = new Vector3[starNumTexts.Length];
            bool isDouble = starKinds.Count % 2 == 0;
            int md = isDouble ? starKinds.Count - 1 : (starKinds.Count - 1) / 2;
            float offest = 1f;
            for (int i = 0; i < starKinds.Count; i++)
            {
                GameObject star = Instantiate(starObject, levelModeStarObject.transform);
                Image starImage = star.GetComponent<Image>();
                starImage.sprite = GameConstManager.Instance.StarSprites[starKinds[i]];
                if (isDouble)
                {
                    star.transform.position = levelModeStarObject.transform.position + new Vector3((2 * i - md) * offest, 0);
                }
                else
                {
                    star.transform.position = levelModeStarObject.transform.position + new Vector3((i - md) * offest, 0);
                }
                star.SetActive(true);
                starNumTexts[starKinds[i]] = star.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
                starNumTexts[starKinds[i]].text = GameManager.Instance.nowLevel.startTargetNum[starKinds[i]].ToString();
            }

            Vector3[] vectors = levelModeStarObject.transform.GetChildPos();
            for (int j = 0; j < starKinds.Count; j++)
            {
                starPos[starKinds[j]] = vectors[j];
            }

            GameManager.Instance.SetStarPos(starPos);
            for (int i = 0; i < vectors.Length; i++)
            {
                Debug.Log(vectors[i]);
            }
        }

        private void OnSubtractStar(object[] args)
        {
        }

        private void OnUpdateSubtractStarUI(object[] args)
        {
            int index = (int)args[0];
            int num = GameManager.Instance.nowLevel.startTargetNum[index];
            Debug.Log(num);
            starNumTexts[index].text = num.ToString();
            if (num <= 0)
            {
                starNumTexts[index].gameObject.SetActive(false);
                starNumTexts[index].transform.parent.Find("gou").gameObject.SetActive(true);
            }
        }

        private void OnScoreWin(object[] args)
        {
            Singleton<ContextManager>.Instance.Push(new ScoreWinViewContext());
        }

        private async void OnStartWin(object[] args)
        {
            await Task.Delay(800);
            Singleton<ContextManager>.Instance.Push(new StarWinViewContext());
        }

        private void OnScoreFailed(object[] args)
        {

        }

        private void OnStarFailed(object[] args)
        {

        }

        public void OnCancelBtnClik()
        {
            GameManager.Instance.ToHome();
            Singleton<ContextManager>.Instance.Push(new LevelViewContext());
            GameManager.Instance.audioController.PlayButtonClickAudio();
        }
    }
}
