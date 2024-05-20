using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class TabInputFieldController : MonoBehaviour
{
    public TMP_InputField[] inputFields; // 포커스 이동할 TMP_InputField들의 배열

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift))
        {
            MoveFocusToPreviousTMP_InputField();
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            MoveFocusToNextTMP_InputField();
        }
    }

    void MoveFocusToNextTMP_InputField()
    {
        // 현재 포커스된 TMP_InputField를 찾기
        TMP_InputField currentTMP_InputField = FindCurrentTMP_InputField();

        // 다음 TMP_InputField로 포커스 이동
        for (int i = 0; i < inputFields.Length; i++)
        {
            if (inputFields[i] == currentTMP_InputField)
            {
                int nextIndex = (i + 1) % inputFields.Length;
                inputFields[nextIndex].Select();
                break;
            }
        }
    }

    void MoveFocusToPreviousTMP_InputField()
    {
        // 현재 포커스된 TMP_InputField를 찾기
        TMP_InputField currentTMP_InputField = FindCurrentTMP_InputField();

        // 이전 TMP_InputField로 포커스 이동
        for (int i = 0; i < inputFields.Length; i++)
        {
            if (inputFields[i] == currentTMP_InputField)
            {
                int previousIndex = (i - 1 + inputFields.Length) % inputFields.Length;
                inputFields[previousIndex].Select();
                break;
            }
        }
    }

    TMP_InputField FindCurrentTMP_InputField()
    {
        foreach (TMP_InputField TMP_InputField in inputFields)
        {
            if (TMP_InputField.isFocused)
            {
                return TMP_InputField;
            }
        }
        return null;
    }

}
