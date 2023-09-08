using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GadgetUIBehaviour : MonoBehaviour
{
    public Image cooldown;
    private bool isCooldown;
    private float lenght;

    private void Update()
    {
        if (isCooldown)
        {
            cooldown.fillAmount -= 1 / lenght * Time.deltaTime;

            if (cooldown.fillAmount <= 0)
            {
                cooldown.fillAmount = 0;
                isCooldown = false;
            }
        }
    }

    public void ReduceCooldown(float amount)
    {
        cooldown.fillAmount -= 1 / lenght * amount;
    }

    public void SetFillAmount(float amount)
    {
        cooldown.fillAmount = amount;
    }

    public void Cooldown(float cooldownDuration)
    {
        cooldown.fillAmount = 1;
        lenght = cooldownDuration;
        isCooldown = true;

        //StartCoroutine(CooldownCoroutine(cooldownDuration));
    }

    private IEnumerator CooldownCoroutine(float duration)
    {
        float time = 0;
        float amountToSubtract = 1 / duration * Time.deltaTime;

        while (time <= duration)
        {
            cooldown.fillAmount -= amountToSubtract;
            time += Time.deltaTime;
            yield return null;
        }
        cooldown.fillAmount = 0;
    }
}
