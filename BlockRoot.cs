using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRoot : MonoBehaviour
{
    public GameObject BlockPrefab = null;//���� ����� ������
    public BlockControl[,] blocks;//�׸���
    void Start()
    {
        
    }

    
    void Update()
    {
          
    }

    public void initialSetUp()//��� ���� ��9x9 ��ġ. SceneControl Ŭ������ ��ŸƮ���� ȣ��� ��
    {
       this.blocks = new BlockControl [Block.BLOCK_NUM_X, Block.BLOCK_NUM_Y];
       int color_index = 0;//����� �� ��ȣ

        for(int y = 0; y < Block.BLOCK_NUM_Y; y++)//ù ����� ������ �����
        {
            for(int x = 0; x < Block.BLOCK_NUM_X; x++)//���ʺ��� �����ʱ���
            {
                //��������� �ν��Ͻ��� ���� ����
                GameObject game_object = Instantiate(this.BlockPrefab) as GameObject;
                //������ ���� ����� BlockControlŬ������ �����´�.
                BlockControl block = game_object.GetComponent<BlockControl>();
                //����� �׸��忡 �����Ѵ�.
                this.blocks[x, y] = block;

                //����� ��ġ ����(�׸��� ��ǥ)�� ����
                block.i_pos.x = x;
                block.i_pos.y = y;
                //�� �����Ʈ���� ������ GameRoot�� �ڽ��̶�� ����
                block.block_root = this;

                //�׸��� ��ǥ�� ���� ��ġ(���� ��ǥ)�� ��ȯ
                Vector3 position = BlockRoot.calcBlockPosition(block.i_pos);
                //���� ��� ��ġ �̵�
                block.transform.position = position;
                //��� �� ����
                block.setColor((Block.COLOR)color_index);
                //��� �̸� ����
                block.name = "block(" + block.i_pos.x.ToString() + "," + block.i_pos.y.ToString() + ")";//���� ��� �� �Ͻ����� �� ���̾��Ű������ ��� ���� Ȯ���� ����
                //��ü �� �߿��� �������� �ϳ��� �� ����
                color_index = Random.Range(0, (int)Block.COLOR.NORMAL_COLOR_NUM);
            }
        }
    }

    public static Vector3 calcBlockPosition(Block.iPosition i_pos)//������ �׸��� ��ǥ�� �������� ��ǥ�� ���Ѵ�
    {
        //��ġ�� ���� �� ���� ��ġ�� �ʱ갪���� ����
        Vector3 position = new Vector3(-(Block.BLOCK_NUM_X / 2.0f - 0.5f),-(Block.BLOCK_NUM_Y / 2.0f -0.5f), 0.0f);

        //�ʱ갪 + �׸�����ǥ * ��� ũ��

        position.x += (float)i_pos.x * Block.COLLISION_SIZE;
        position.y += (float)i_pos.y * Block.COLLISION_SIZE;

        return (position);//�������� ��ǥ�� ��ȯ
    }
}
