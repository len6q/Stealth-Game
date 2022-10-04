using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DefenderHud : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private Image _fill;
    [SerializeField] private Gradient _gradient;

    [SerializeField] private Transform _panel;
    [SerializeField] private TextMeshProUGUI _mainText;    
    [SerializeField] private Button _button;    

    private const string MAIN_SCENE = "Main";
    private const string MENU_SCENE = "Menu";    

    public void Init(int maxNoise)
    {        
        InitNoiseBar(maxNoise);
        InitPanel();
    }

    private void InitNoiseBar(int maxNoise)
    {
        _slider.maxValue = maxNoise;
        _slider.value = 0;
        _fill.color = _gradient.Evaluate(_slider.value);
    }

    public void SetNoiseIndicator(int noise)
    {
        _slider.value = noise;
        _fill.color = _gradient.Evaluate(_slider.normalizedValue);
    }

    private void InitPanel()
    {        
        _panel.gameObject.SetActive(false);
        _button.onClick.AddListener(LoadScene);
    }

    private void LoadScene()
    {
        SceneLoader.Load(MENU_SCENE);
    }

    public void ShowPanel(bool isDefeat)
    {
        _panel.gameObject.SetActive(true);
        _mainText.text = isDefeat ? "defeat" : "winner";        
    }

    public void LoadMain()
    {
        SceneLoader.Load(MAIN_SCENE);
    }
}
