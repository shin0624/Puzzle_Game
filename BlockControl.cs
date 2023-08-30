using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Block;

public class Block
{//��Ͽ� ���� ������ �ٷ�� Ŭ����.
    public static float COLLISION_SIZE = 1.0f;//����� �浹 ũ��
    public static float VANISH_TIME = 3.0f;//���� �ٰ� ����� �������� �ð�

   

    public struct iPosition 
    {//�׸��忡���� ��ǥ�� ��Ÿ���� ����ü
        public int x;
        public int y;
    }
    public enum COLOR
    {
        //��ϻ��� ����
        NONE = -1,//������ X
        PINK = 0,
        BLUE,
        YELLOW,
        GREEN,
        MAGENTA,
        ORANGE,
        GRAY,
        NUM,//�÷� ���� = 7
        FIRST = PINK,//�ʱ� �÷�
        LAST = ORANGE,//���� �÷�
        NORMAL_COLOR_NUM = GRAY,//���� �÷�(ȸ�� �̿��� ��)�� ��
    };

    public enum DIR4
    {//�����¿� �� ����
        NONE = -1,//��������X
        RIGHT,
        LEFT,
        UP,
        DOWN,
        NUM,//���� ����  =4
    };

    public enum STEP
    {//����� ���� ǥ��
        NONE = -1,//�������� ����
        IDLE = 0,//�����
        GRABBED,//��������
        RELEASED,//������ ����
        SLIDE,//�����̵� ��
        VACANT,//�Ҹ� ��
        RESPAWN,//����� ��
        FALL,//���� ��
        LONG_SLIDE,//ũ�� �����̵� ��
        NUM,//���°� ���������� ǥ��
    };


    public static int BLOCK_NUM_X = 9;//����� ��ġ�� �� �ִ� X���� �ִ� ��
    public static int BLOCK_NUM_Y = 9;//����� ��ġ�� �� �ִ� Y���� �ִ� ��
}

public class BlockControl : MonoBehaviour
{
    public Block.COLOR color = (Block.COLOR)0;//��� �� �ʱ� ->��ũ
    public BlockRoot block_root = null;//��Ϸ�Ʈ Ŭ������ ����.����� �����ų� ��ü
    public Block.iPosition i_pos;//��� ��ǥ

    public Block.STEP step = Block.STEP.NONE;//���� ����
    public Block.STEP next_step = Block.STEP.NONE;//���� ����
    private Vector3 position_offset_initial = Vector3.zero;// ��ü �� ��ġ
    public Vector3 position_offset = Vector3.zero;//��ü �� ��ġ

    public float vanish_timer = -1.0f;//����� ����� �� ������ �ð�
    public Block.DIR4 slide_dir = Block.DIR4.NONE;//�����̵� �� ����.
    public float step_timer = 0.0f;//����� ��ü�� ���� �̵��ð� 

    public Block.DIR4 calcSlideDir(Vector2 mouse_position)//�μ� ���콺 ��ġ�� �������� ��������� �����̵�Ǿ����� �Ǵ� �� Block.DIR4�� ������ ��ȯ-->��� ��ü ���θ� �Ǵ�.
    {
        Block.DIR4 dir = Block.DIR4.NONE;
        Vector2 v = mouse_position - new Vector2(this.transform.position.x, this.transform.position.y);//������ ���콺 ��ġ�� ���� ��ġ�� ��

        if (v.magnitude > 0.1f)//magnitude = ���� ������ ���� ����. �� ������ ũ��
        {
            //���� ũ�� 0.1 �����̸� �����̵����� ���� ������ ����
            if (v.y > v.x)//v�� 0.1 �̻��̸� ������ ���ϰ� DIR4�� �� ��ȯ
            {
                if (v.y > -v.x)
                {
                    dir = Block.DIR4.UP;
                }
                else
                {
                    dir = Block.DIR4.LEFT;
                }
            }
            else
            {
                if (v.y > -v.x)
                {
                    dir = Block.DIR4.RIGHT;
                }
                else
                {
                    dir = Block.DIR4.DOWN;
                }
            }
        }
        return (dir);
    }

    public float calcDirOffset(Vector2 position, Block.DIR4 dir)
    {
        //�μ�(��ġ, ����)�� �ٰŷ�, ���� ��ġ�� �����̵� �� ���� �Ÿ��� ����������� ��ȯ
        float offset = 0.0f;
        Vector2 v = position - new Vector2(this.transform.position.x, this.transform.position.y);//������ ��ġ�� ����� ���� ��ġ�� ��
        switch (dir)
        {
            case Block.DIR4.RIGHT: offset = v.x; break;
            case Block.DIR4.LEFT: offset = -v.x; break;
            case Block.DIR4.UP: offset = v.y; break;
            case Block.DIR4.DOWN: offset = -v.y; break;
        }
        return (offset);
    }

    public void beginSlide(Vector3 offset)
    {
        this.position_offset_initial = offset;
        this.position_offset = this.position_offset_initial;
        this.next_step = Block.STEP.SLIDE;//���¸� SLIDE�� ����
    }

    void Start()
    {
        this.setColor(this.color);//��ĥ
        this.next_step = Block.STEP.IDLE;//���� ����� ���������
    }

    
    void Update()
    {
        Vector3 mouse_position;//���콺 ��ġ
        this.block_root.unprojectMousePosition(out mouse_position, Input.mousePosition);//���콺 ��ġ ȹ��. unprojectMousePosition = BlockRootŬ������ �޼��忡��, ���콺�� ���� ��� ����� ǥ���� ����Ű���� ���

        //ȹ���� ���콺 ��ġ�� x�� y������ �Ѵ�.
        Vector2 mouse_position_xy = new Vector2(mouse_position.x, mouse_position.y);

        this.step_timer += Time.deltaTime;
        float slide_time = 0.2f;

        if(this.next_step== Block.STEP.NONE)//�������� ������ ���
        {
            switch (this.step) 
            {
                case Block.STEP.SLIDE: if (this.step_timer >= slide_time)
                    {
                        if (this.vanish_timer == 0.0f)
                        {
                            this.next_step = Block.STEP.VACANT;//�����̵� ���� ��� �Ҹ� �� VACANT(�����)���·� ����
                        }
                        else
                        {
                            this.next_step = Block.STEP.IDLE;
                        }
                    }
                    break;
            }

        }


        //���� ��� ���°� "���� ����" �̿��� ���� = �� , ���� ��� ���°� ����� ���
        while (this.next_step != Block.STEP.NONE)
        {
            this.step = this.next_step;
            this.next_step = Block.STEP.NONE;

            switch(this.step)
            {
                case Block.STEP.IDLE://��� ����
                    this.position_offset = Vector3.zero;//��� ǥ�� ũ�⸦ ���� ũ���
                    this.transform.localScale = Vector3.one * 1.0f;
                    break;
                case Block.STEP.GRABBED://���� ����
                    this.transform.localScale = Vector3.one * 1.2f;//��� ǥ�� ũ�⸦ ũ��
                    break;
                case Block.STEP.RELEASED://������ �ִ� ����
                    this.position_offset = Vector3.zero;
                    this.transform.localScale = Vector3.one * 1.0f;//��� ǥ�� ũ�⸦ ���� �������
                    break;
                case Block.STEP.VACANT://����� ����
                    this.position_offset= Vector3.zero;
                    break;
            }
            this.step_timer = 0.0f;
        }
        switch (this.step)
        {
            case Block.STEP.GRABBED://���� ������ ��. �׻� �����̵� ������ üũ�ϵ���
                this.slide_dir = this.calcSlideDir(mouse_position_xy); break;
            case Block.STEP.SLIDE://�����̵� ���� ��.
                float rate = this.step_timer / slide_time;//����� ������ �̵��ϴ� �Ÿ�.
                rate = Mathf.Min(rate, 1.0f);
                rate = Mathf.Sin(rate * Mathf.PI / 2.0f);
                this.position_offset = Vector3.Lerp(this.position_offset_initial, Vector3.zero, rate);break;
                //Vector3.Lerp = ���ۺ��� ~ ��ǥ���� ������ �ð��� ���� ��ġ�� ���� �� ���.Lerp(start, finish, 0.0~1.0��)
        }

        //�׸��� ��ǥ�� ���� ��ǥ(���� ��ǥ)�� ��ȯ�ϰ� position_offset �߰�
        Vector3 position = BlockRoot.calcBlockPosition(this.i_pos) + this.position_offset;

        //���� ��ġ�� ���ο� ��ġ�� ����
        this.transform.position = position;

    }
    public void beginGrab()
    {
        this.next_step = Block.STEP.GRABBED;
    }

    public void endGrab()
    {
        this.next_step = Block.STEP.IDLE;
    }
    public bool isGrabbable()
    {
        bool is_grabbable = false;
        switch (this.step) 
        {
            case Block.STEP.IDLE:is_grabbable = true;break;//��� ������ ������ true(���� �� �ִ�)�� ��ȯ
        }
        return(is_grabbable);
    }

    public bool isContainedPosition(Vector2 position)
    {
        bool ret = false;
        Vector3 center = this.transform.position;
        float h = Block.COLLISION_SIZE / 2.0f;
        /*do
        {
            //x��ǥ�� �ڽŰ� ��ġ�� ������ break�� ���� Ż��
            if (position.x < center.x - h || center.x + h < position.x)
            {
                break;
            }
            //y��ǥ�� �ڽŰ� ��ġ�� ������ break�� ���� Ż��
            if (position.y < center.y - h || center.y + h < position.y)
            {
                break;
            }
            //x��ǥ, y��ǥ ��� ���������� true(���� �ִ�)�� ��ȯ
            ret = true;
        } while (false);*/
        if (position.x >= center.x - h && position.x <= center.x + h && position.y >= center.y - h && position.y <= center.y + h)
        {//����� �߽ɰ� ���콺 ��ǥ ���� �Ÿ��� ����Ͽ� �ش� �Ÿ��� ����� ũ�� �ݸ�ŭ �̳��� ��쿡�� Ŭ�� ������ �������� �Ǵ�.
         //-->����� �߽ɿ��� �󸶳� ������ �ִ����� ���� ���� ��Ȯ�� ������ �Ǵܰ���
            ret = true;
        }
        return (ret);
    }



    public void setColor(Block.COLOR color)
    {//�μ� color�� ������ ����� ĥ�Ѵ�.
        this.color = color;//������ ���� ��� ������ ����
        Color color_value;//ColorŬ������ ���� ��Ÿ��.

        switch (this.color)//ĥ�� ���� ���� ���̽� ����
        {
            default:
            case Block.COLOR.PINK:
                color_value = new Color(1.0f, 0.5f, 0.5f);
                break;

            case Block.COLOR.BLUE:
                color_value = Color.blue;
                break;

            case Block.COLOR.YELLOW:
                color_value = Color.yellow;
                break;
            case Block.COLOR.GREEN:
                color_value = Color.green;
                break;
            case Block.COLOR.MAGENTA:
                color_value = Color.magenta;
                break;
            case Block.COLOR.ORANGE:
                color_value = new Color(1.0f, 0.46f, 0.0f);
                break;
        }
        //�� ���� ������Ʈ�� ���׸��� ���� ����
        this.GetComponent<Renderer>().material.color = color_value;
    }
}
