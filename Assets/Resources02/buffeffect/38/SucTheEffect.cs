using UnityEngine;

public class SucTheEffect : BuffEffect
{
    [SerializeField] private GameObject sucTheAtk;

    void Start()
    {
    }

    public void ShowSucTheAtk()
    {
        sucTheAtk.SetActive(false);
        sucTheAtk.SetActive(true);
        var listPar = sucTheAtk.transform.GetComponentsInChildren<ParticleSystem>();
        foreach (var par in listPar)
        {
            par.Play();
        }
    }
}