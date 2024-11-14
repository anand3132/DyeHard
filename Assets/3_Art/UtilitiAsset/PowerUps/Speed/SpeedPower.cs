using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SpeedPower : MonoBehaviour
{
  public GameObject SpeedRun;
  public GameObject SlowRun;
    

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SpeedPowerUps();
        }
        if(Input.GetKeyUp(KeyCode.V))
        {
            SludgePowerUps();
        }
        
    }
    public void SpeedPowerUps()
    {
      this.transform.DOKill(true);
      SlowRun.SetActive(false);
      SpeedRun.SetActive(true);
      this.transform.DOMoveZ(this.transform.position.z + 10f, 2.5f).SetEase(Ease.Linear,3).SetDelay(0.1f).OnComplete(()=>
      {
        this.transform.GetComponent<Animator>().SetTrigger("StopRun");    
      });
      this.transform.GetComponent<Animator>().SetTrigger("SpeedRun");
    }
    public void SludgePowerUps()
    {
       this.transform.DOKill(true);
      SlowRun.SetActive(true);
      SpeedRun.SetActive(false);
      this.transform.DOMoveZ(this.transform.position.z + 10f, 10f).SetEase(Ease.Linear,3).SetDelay(0.1f).OnComplete(()=>
      {
        this.transform.GetComponent<Animator>().SetTrigger("StopRun");    
      });
      this.transform.GetComponent<Animator>().SetTrigger("SlowRun");
    }
}
