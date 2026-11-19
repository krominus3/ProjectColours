using UnityEngine;

public class LensManager : MonoBehaviour
{
    [SerializeField] private LensInventory inventory;
    [SerializeField] private Material fullscreenMaterial;

    private void Awake()
    {
        if (inventory == null)
        {
            Debug.LogError("Не подключен инвентарь!");
            inventory = FindAnyObjectByType<LensInventory>();
        }

        // Поставить стандарт (сброс экрана при старте)
        if (fullscreenMaterial != null)
        {
            fullscreenMaterial.SetVector("_rgba", new Vector4(0f, 0f, 0f, 1f));
        }
    }

    private void OnEnable()
    {
        if (inventory != null)
        {
            // Подписываемся на ОДНО событие
            inventory.OnLensChanged += OnLensChanged;
        }
    }

    private void OnDisable()
    {
        if (inventory != null)
        {
            inventory.OnLensChanged -= OnLensChanged;
        }
    }

    // Единый метод, который делает всё
    private void OnLensChanged(LensType type)
    {
        ChangeColour(type);

        //// Проходимся по заранее собранному списку
        //foreach (var block in ColoredBlock.AllBlocks)
        //{
        //    block.ReactToLens(type);
        //}
    }

    public void ChangeColour(LensType type)
    {
        if (fullscreenMaterial == null) return;

        if (type == LensType.None) fullscreenMaterial.SetVector("_rgba", new Vector4(0f, 0f, 0f, 1f));
        else if (type == LensType.Red) fullscreenMaterial.SetVector("_rgba", new Vector4(1f, 0f, 0f, 1f));
        else if (type == LensType.Green) fullscreenMaterial.SetVector("_rgba", new Vector4(0f, 1f, 0f, 1f));
        else if (type == LensType.Blue) fullscreenMaterial.SetVector("_rgba", new Vector4(0f, 0f, 1f, 1f));
        else if (type == LensType.Cyan) fullscreenMaterial.SetVector("_rgba", new Vector4(-1f, 0f, 0f, 1f));
        else if (type == LensType.Purple) fullscreenMaterial.SetVector("_rgba", new Vector4(0f, -1f, 0f, 1f));
        else if (type == LensType.Yellow) fullscreenMaterial.SetVector("_rgba", new Vector4(0f, 0f, -1f, 1f));
        else if (type == LensType.Orange) fullscreenMaterial.SetVector("_rgba", new Vector4(0.5f, 0f, -0.5f, 1f));
        else if (type == LensType.White) fullscreenMaterial.SetVector("_rgba", new Vector4(1f, 1f, 1f, 1f));
    }
}