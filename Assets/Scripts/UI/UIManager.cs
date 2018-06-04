using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public Canvas LifeCanvas;

    public TextMeshProUGUI DaysPassedText;
    public TextMeshProUGUI HoursePassedText;

    public Button CasernBuyButton;
    public Button HospitalBuyButton;
    public Button StopperBuyButton;

    public Button SoldierBuyButton;
    public Button MedicBuyButton;

    public GameObject PopHeader;
    public GameObject Pop;

    public GameObject BuildingsPanel;
    public GameObject CasernPanel;
    public GameObject HospitalPanel;

    public TextMeshProUGUI LivingsValueText;
    public TextMeshProUGUI InfectedsValueText;
    public TextMeshProUGUI UndeadsValueText;
    public TextMeshProUGUI DeadsValueText;

    public TextMeshProUGUI MoneyText;

    public TextMeshProUGUI SoldierRankText;
    public TextMeshProUGUI MedicRankText;

    public Image PausedImage;

    public Slider SliderSpeed;

    public GameObject GameOverPanel;

    public GameObject QuitPanel;

    public GameObject WonPanel;
}
