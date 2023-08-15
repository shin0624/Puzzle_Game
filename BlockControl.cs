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

    public static int BLOCK_NUM_X = 9;//����� ��ġ�� �� �ִ� X���� �ִ� ��
    public static int BLOCK_NUM_Y = 9;//����� ��ġ�� �� �ִ� Y���� �ִ� ��
}

public class BlockControl : MonoBehaviour
{
    public Block.COLOR color = (Block.COLOR)0;//��� �� �ʱ� ->��ũ
    public BlockRoot block_root = null;//��Ϸ�Ʈ Ŭ������ ����.����� �����ų� ��ü
    public Block.iPosition i_pos;//��� ��ǥ

    
    void Start()
    {
        this.setColor(this.color);//��ĥ
    }

    
    void Update()
    {
        
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
