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
            }
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
