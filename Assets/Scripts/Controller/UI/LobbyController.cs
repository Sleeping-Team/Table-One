using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyController : MonoBehaviour
{
    [SerializeField] private TMP_Text _lobbyCode;
    
    [SerializeField] private Button continueButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button NotconfirmButton;
    
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject confirmGameObjectl;
    
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerData.Instance.lobbyCode == null)
        {
            _lobbyCode.text = PlayerData.Instance.joinCode;
        }
        else
        {
            _lobbyCode.text = PlayerData.Instance.lobbyCode;
        }

        Button continueBtn = continueButton.GetComponent<Button>();
        Button confirmBtn = confirmButton.GetComponent<Button>();
        Button exitBtn = exitButton.GetComponent<Button>();
        Button NotconfirmBtn = NotconfirmButton.GetComponent<Button>();
        continueBtn.onClick.AddListener(ContinueButtonOnClick);
        confirmBtn.onClick.AddListener(confirmButtomOnClick);
        NotconfirmBtn.onClick.AddListener(NoconfirmButtomOnClick);
        exitBtn.onClick.AddListener(ExitButtonOnClick);
        
        //pauseUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (_lobbyCode.text == null)
        {
            if (PlayerData.Instance.lobbyCode == null)
            {
                _lobbyCode.text = PlayerData.Instance.joinCode;
            }
            else if(PlayerData.Instance.joinCode == null)
            {
                _lobbyCode.text = PlayerData.Instance.lobbyCode;
            }
            
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseUI.SetActive(true);
        }
    }

    public void Pause()
    {
        pauseUI.SetActive(true);
    }

    public void confirmButtom()
    {
        confirmGameObjectl.SetActive(true);
    }
    public void ContinueButtonOnClick()
    {
        //check the button is clicked
        Debug.Log("You have clicked the continue button!");
        
        pauseUI.SetActive(false);
        confirmGameObjectl.SetActive(false);
    }

    public void confirmButtomOnClick()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void NoconfirmButtomOnClick()
    {
        confirmGameObjectl.SetActive(false);
        pauseUI.SetActive(true);
    }
    
    public void ExitButtonOnClick()
    {
        //check the button is clicked
        Debug.Log ("You have clicked the exit button!");
        pauseUI.SetActive(false);
        confirmGameObjectl.SetActive(true);
    }
}
