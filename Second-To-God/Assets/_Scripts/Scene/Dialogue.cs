using UnityEngine;
using UnityEngine.UIElements;

public class Dialogue : MonoBehaviour
{
	[SerializeField] private UIDocument uIDocument;
	private TextElement textElement;
	private Button response1, response2;

	void Awake()
	{
		var root = uIDocument.rootVisualElement;
		textElement = root.Q<TextElement>("DialogueText");
		response1 = root.Q<Button>("Response1");
		response2 = root.Q<Button>("Response2");

		response1.clicked += () => OnResponseSelected(1);
		response2.clicked += () => OnResponseSelected(2);
	}

	void OnDisable()
	{
		response1.clicked -= () => OnResponseSelected(1);
		response2.clicked -= () => OnResponseSelected(2);
	}

	private void OnResponseSelected(int responseIndex)
	{
		// Handle response selection logic here
	}

	private void SetDialogueText(string text)
	{
		textElement.text = text;
	}
}
