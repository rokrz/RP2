using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Text nameText;
    public Text dialogueText;
    public Animator animator;
    private Queue<string> sentences;
    public static DialogueManager instance = null;
    public bool isDialoguing { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        isDialoguing = false;
        sentences = new Queue<string>();
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public void StartDialogue(Dialogue dialogue)
    {
        isDialoguing = true;
        sentences = new Queue<string>();
        animator.SetBool("isOpen",true);
        sentences.Clear();

        nameText.text = dialogue.name;

        foreach(string sentence in dialogue.sentences){
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();

    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char c in sentence.ToCharArray())
        {
            dialogueText.text += c;
            yield return null;
        }
    }

    public void EndDialogue()
    {
        isDialoguing = false;
        animator.SetBool("isOpen", false);
    }
}
