using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextChange : MonoBehaviour
{
    [SerializeField] float _countDownTime = 5f;

    [Header("Questions")]
    [SerializeField] int _numberOfQuestions = 10;
    [SerializeField] float _minQuestionTime = 2f;
    [SerializeField] float _maxQuestionTime = 8f;

    [Tooltip("The color of a question that has maximum time remaining.")]
    [SerializeField] Color _newQuestionColor;
    [Tooltip("The color of a question that has minimum time remaining.")]
    [SerializeField] Color _oldQuestionColor;

    [SerializeField] List<string> _questions;

    [Header("References")]
    [SerializeField] GameObject _menuPanel;
    [SerializeField] Text _gameText;

    Stack<string> _questionOrder = new Stack<string>();
    Gradient _questionGradient;

	private void Start()
	{
		if (_numberOfQuestions > _questions.Count + 1)
		{
            throw new System.ArgumentOutOfRangeException(nameof(_numberOfQuestions), "Number of Questions must be less than or equal to the count of Questions.");
		}
	}

	void StartGame()
    {
        // We want to ensure that 'what is your name' is always a question in the list.
        int nameIndex = Random.Range(0, _numberOfQuestions);

        for (int i=0; i<_numberOfQuestions; i++)
		{
            int randInd = Random.Range(0, _questions.Count);
            string question = (i == nameIndex) ? "What is your name?" : _questions[randInd];

            if (randInd != nameIndex)
                _questions.RemoveAt(randInd);

            _questionOrder.Push(question);

        }

        // establish the timing gradient
        _questionGradient = new Gradient();
        GradientColorKey[] colorKeys = new GradientColorKey[2];
        colorKeys[0].color = _newQuestionColor;
        colorKeys[0].time = 0f;
        colorKeys[1].color = _oldQuestionColor;
        colorKeys[1].time = 1f;

        _questionGradient.colorKeys = colorKeys;

        // Start generating questions
        StartCoroutine(nameof(GenerateQuestions));
    }

    IEnumerator GenerateQuestions()
	{
        while (_questionOrder.Count > 0)
		{
            float questionTime = Random.Range(_minQuestionTime, _maxQuestionTime);
            string question = _questionOrder.Pop();

            _gameText.text = question;

            float elapsedTime = 0f;

            while (questionTime > elapsedTime)
            {
                _gameText.color = _questionGradient.Evaluate(elapsedTime/questionTime);

                yield return new WaitForSecondsRealtime(0.1f);
                elapsedTime += 0.1f;
            }
		}


        _gameText.color = Color.white;
        _gameText.text = "The game is over!";
	}

    public void Button_StartGame()
	{
        _menuPanel.SetActive(false);
        _gameText.gameObject.SetActive(true);

        StartCoroutine(nameof(TimerCountdown));
	}

    IEnumerator TimerCountdown()
	{
        float timeSpent = 0f;
        while (timeSpent < _countDownTime)
		{
            yield return new WaitForSecondsRealtime(0.01f);
            timeSpent += 0.01f;

            _gameText.text = (((_countDownTime - timeSpent)*100 + 0.01)/100).ToString().Substring(0,3);
        }

        StartCoroutine(nameof(StartGame));
	}

}
