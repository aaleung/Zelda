using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction { NORTH, EAST, SOUTH, WEST };
public enum EntityState { NORMAL, ATTACKING };

public class player_control : MonoBehaviour
{

    public float walking_velocity = 1.0000f;
    public int rupee_count = 0;

    //6 HP = 3 full hearts
    public float max_HP = 6;
    public float HP = 6;

    public float damage_cd = 0.0f;

    public float TILE_SIZE = 0.50f;
    public float HALF_TILE_SIZE = 0.225f;

    public Sprite[] link_run_down;
    public Sprite[] link_run_up;
    public Sprite[] link_run_right;
    public Sprite[] link_run_left;

    StateMachine animation_state_machine;
    StateMachine control_state_machine;

    public EntityState current_state = EntityState.NORMAL;
    public Direction current_direction = Direction.SOUTH;

    public GameObject selected_weapon_prefab;

    public static player_control instance;

    // Use this for initialization
    void Start()
    {
        if (instance != null)
        {
            Debug.LogError("Multiple Link objects detected! There is more than one Link.");
        }
        instance = this; //Singleton

        animation_state_machine = new StateMachine();
        animation_state_machine.ChangeState(new StateIdleWithSprite(this, GetComponent<SpriteRenderer>(), link_run_down[0]));

    }

    // Update is called once per frame
    void Update()
    {
        //If damage cooldown is on, keep ticking the time down. If damage_cd is 0, Link can be hurt again
        if (damage_cd > 0)
        {
            damage_cd -= Time.deltaTime;
        }

        animation_state_machine.Update();

        walking_velocity = Time.deltaTime;
        walking_velocity *= 30.0f;
        HandleMovement();

        
    }

    void HandleMovement()
    {
        float horizontal_input = Input.GetAxis("Horizontal");
        float vertical_input = Input.GetAxis("Vertical");


        //Trying to replicate the original priority movement, but might not be necessary, and also seems like Unity won't let this
        //If up or down are held, and while you're still holding you press left or right, it will prioritize the vertical direction and ignore horizontal
        //If left or right are held, and while you're still holding you press up or down, it will ignore the left/right keys being held and change to up or down



        //Where the player input should move the player, without movement correction
        Vector3 walk = new Vector3(horizontal_input, vertical_input, 0) * walking_velocity;

        //Direction player ultimately wants to go; set to current direction for default for debugging
        Direction desired_direction = current_direction;
        if (walk.y > 0.0f)
        {
            walk.x = 0.0f;
            desired_direction = Direction.NORTH;
            Debug.Log("DESIRED: " + desired_direction);
        }
        else if (walk.y < 0.0f)
        {
            walk.x = 0.0f;
            desired_direction = Direction.SOUTH;
            Debug.Log("DESIRED: " + desired_direction);
        }
        else if (walk.y == 0.0f && walk.x > 0.0f)
        {
            desired_direction = Direction.EAST;
            Debug.Log("DESIRED: " + desired_direction);
        }
        else if (walk.y == 0.0f && walk.x < 0.0f)
        {
            desired_direction = Direction.WEST;
            Debug.Log("DESIRED: " + desired_direction);
        }

        //Debug.Log("Vertical: " + walk.y);
        //Debug.Log("Horizontal: " + walk.x);


        //Begin movement correction; if Link wants to move on a different axis, he must be re-aligned first on the axis
        if ((current_direction == Direction.NORTH || current_direction == Direction.SOUTH) &&
            (desired_direction == Direction.WEST || desired_direction == Direction.EAST))
        {
            if ((GetComponent<Transform>().position.y % TILE_SIZE < 0.22) && (GetComponent<Transform>().position.y % TILE_SIZE > 0.03))
            {
                current_direction = Direction.SOUTH;
                walk = new Vector3(0, -.5000f, 0) * walking_velocity;
            }
            else if (GetComponent<Transform>().position.y % TILE_SIZE > 0.28)
            {
                current_direction = Direction.NORTH;
                walk = new Vector3(0, .5000f, 0) * walking_velocity;
            }
            //Player is changing axis, but needs no correction
            else
            {
                current_direction = desired_direction;
            }
        }
        else if ((current_direction == Direction.WEST || current_direction == Direction.EAST) &&
                (desired_direction == Direction.NORTH || desired_direction == Direction.SOUTH))
        {
            if ((GetComponent<Transform>().position.x % TILE_SIZE < 0.22) && (GetComponent<Transform>().position.x % TILE_SIZE > 0.03))
            {
                current_direction = Direction.WEST;
                walk = new Vector3(-.5000f, 0, 0) * walking_velocity;
            }
            else if (GetComponent<Transform>().position.x % TILE_SIZE > 0.28)
            {
                current_direction = Direction.EAST;
                walk = new Vector3(.5000f, 0, 0) * walking_velocity;
            }
            //Player is changing axis, but needs no correction
            else
            {
                current_direction = desired_direction;
            }

        }
        //Player is changing directions but not axis; no correcting should be done
        else
        {
            current_direction = desired_direction;
        }

        GetComponent<Rigidbody>().velocity = walk;


    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "Rupee")
        {
            Destroy(coll.gameObject);
            rupee_count++;
        }
        //If collider was an enemy hitbox and damage cooldown is off, take damage
        if (coll.gameObject.tag == "Enemy" && damage_cd == 0)
        {
            HP--;
            //activate damage cooldown
            damage_cd = 0.8f;
            //activate damage flashing
            //DamageInvincibility();
            if (HP <= 0)
            {
                GameOver();
            }
        }
        
    }

    void GameOver()
    {
        //Link death animation
        //Restart dungeon
    }
}
