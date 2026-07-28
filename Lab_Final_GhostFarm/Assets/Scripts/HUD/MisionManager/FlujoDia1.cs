using System.Collections.Generic;
using UnityEngine;

public class FlujoDia1 : MonoBehaviour
{
    void Start()
    {
        MisionManager.Instance.IniciarMisionPrincipal(new List<Objetivo> {
            new Objetivo { id = MisionId.RecogerHoz, descripcion = "Recoger la hoz" }
        });
    }
}