using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Sprite[] playerSprites = new Sprite[4];
    private SpriteRenderer currentSprite;
    private Transform tr;
    public float speed = 50f;
    // Start is called before the first frame update
    void Start()
    {
        currentSprite = GetComponent<SpriteRenderer>();
        tr = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        float xAxis = Input.GetAxisRaw("Horizontal");
        float zAxis = Input.GetAxisRaw("Vertical");

        Vector3 move = new Vector3(xAxis, 0f, zAxis);

        tr.Translate(move * speed * Time.deltaTime, Space.World);
        ChangeSprite();

        // if (Input.GetKey(KeyCode.LeftShift))
        // {
        //     Debug.Log("button is pressed");
        //     speed *= 2;
        // } 
        // else
        // {
        //     speed = 50;
        // }
    }

    public void ChangeSprite()
    {
        if (Input.GetKeyDown("w") || Input.GetKeyDown(KeyCode.UpArrow))
        {
            
            currentSprite.sprite = playerSprites[0];
        }

        if (Input.GetKeyDown("a") || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            
            currentSprite.sprite = playerSprites[1];
        }

        if (Input.GetKeyDown("s") || Input.GetKeyDown(KeyCode.DownArrow))
        {
            
            currentSprite.sprite = playerSprites[2];
        }

        if (Input.GetKeyDown("d") || Input.GetKeyDown(KeyCode.RightArrow))
        {
            
            currentSprite.sprite = playerSprites[3];
        }
    }
}
