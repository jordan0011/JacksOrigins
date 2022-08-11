using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DarkEssenceText : MonoBehaviour
{
    [SerializeField]
    Text damageText1;
    [SerializeField]
    Text damageText2;

    [SerializeField]
    Text newText1;
    [SerializeField]
    Text newText2;

    private float timer = 0;
    private float x = 0f, y;
    private Vector3 startposition;
    private int fontsize;
    private int fontstartsize;
    [SerializeField]
    private bool Essence = false;
    [SerializeField]
    private bool Experience = false;
    // Start is called before the first frame update
    void Start()
    {
        startposition = transform.position;
        fontsize = damageText1.fontSize;
        fontstartsize = fontsize * 2;

        damageText1.fontSize = fontstartsize;
        damageText2.fontSize = fontstartsize;

        newText1.fontSize = fontstartsize;
        newText2.fontSize = fontstartsize;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (fontstartsize > fontsize)
        {
            int smallrate = 10;
            fontstartsize -= smallrate;
            damageText1.fontSize = fontstartsize;
            damageText2.fontSize = fontstartsize;
            newText1.fontSize = fontstartsize;
            newText2.fontSize = fontstartsize;
        }


        // if(timer > 0.18f)
        // {
        if(!Essence && !Experience)
        {
        y = 0.2f * Mathf.Sin((timer + 0.2f) * 3f);

        }
        else
        {
            y = 0.2f *timer;
        }
        transform.position = new Vector3(0, y, 0) + startposition;

        if (timer > 0.2f && !Essence && !Experience)
        {
            Color alpha = damageText1.color;
            Color alpha1 = damageText2.color;

            float vanishingRate = Time.deltaTime;
            alpha = new Color(alpha.r, alpha.b, alpha.g, alpha.a - vanishingRate);
            alpha1 = new Color(alpha1.r, alpha1.b, alpha1.g, alpha1.a - vanishingRate);

            damageText1.color = alpha;
            damageText2.color = alpha1;
        }
    }
    public void SetDarkEssenceValue(int value, float scale)
    {
        damageText1.text = value.ToString();
        damageText2.text = value.ToString();
        if (Experience)
        {
            damageText1.text += " XP";
            damageText2.text += " XP";
        }

        scale = scale * 0.002f;
        transform.localScale = new Vector3(scale, scale, scale);
    }
}/*public Text start;
    public Text end;

    public GameObject text;
    public GameObject shadow;

    private float timer = 0;

    private Vector3 textstartposition;
    private Vector3 shadowstartposition;

    private float x;
    // Start is called before the first frame update
    void Start()
    {
        textstartposition = text.transform.position;
        shadowstartposition = shadow.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        end.text = start.text;
        timer += Time.deltaTime;

        //if (timer >0.18f)
        // {
        x = 0.5f * Mathf.Sin((timer + 0.2f) * 3f) ;
            text.transform.position = new Vector3(0, x, 0) + textstartposition;
            shadow.transform.position = new Vector3(0, x, 0) + shadowstartposition;
        //}
    }*/
