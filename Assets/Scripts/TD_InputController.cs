using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TD_InputController : MonoBehaviour
{
    public struct Dragging {
        public bool isDragging;
        public Vector2 start_pos;
        public Vector2 end_pos;
    }

    static public Dragging dragging;

    // Start is called before the first frame update
    void Start()
    {
        dragging.isDragging = false;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDragging();
    }

    void UpdateDragging() {
        if (CursorClickDown()) {
            dragging.isDragging = true;
            dragging.start_pos = Camera.main.ScreenToWorldPoint(CursorPosition());
        }


        if (CursorClickUp()) {
            dragging.isDragging = false;
            dragging.end_pos = Camera.main.ScreenToWorldPoint(CursorPosition());
        }
    }

    bool CursorClickDown() {
        return Input.GetMouseButtonDown(0);
    }

    bool CursorClickUp() {
        return Input.GetMouseButtonUp(1);
    }

    Vector3 CursorPosition() {
        return Input.mousePosition;
    }
}
