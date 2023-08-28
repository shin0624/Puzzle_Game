using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRoot : MonoBehaviour
{
    public GameObject BlockPrefab = null;//���� ����� ������
    public BlockControl[,] blocks;//�׸���

    private GameObject main_camera = null;//���� ī�޶�
    private BlockControl grabbed_block = null;//���� ���

    void Start()
    {
        //ī�޶�κ��� ���콺 Ŀ���� ����ϴ� ������ ��� ���� ���� ī�޶� Ȯ��-->������ ��� ǥ�鿡 ���� ��� ��� ����Ű���� ����ϱ� ���� ī�޶� ������Ʈ�� �����´�
        this.main_camera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    
    void Update()
    {
        //9x9 ��� �迭 �� ��� ��Ͽ� ���Ͽ�, ���콺 ��ǥ�� ��ġ���� üũ, ���� �� �ִ� ������ ����� �⵵�� ó��
        Vector3 mouse_position;//���콺 ��ġ
        this.unprojectMousePosition(out mouse_position, Input.mousePosition);//���콺 ��ġ�� �����´�
        //������ ���콺 ��ġ�� �ϳ��� Vector2�� ������.
        Vector2 mouse_position_xy = new Vector2(mouse_position.x, mouse_position.y);
        if(this.grabbed_block == null)//���� ����� �������
        {
            //if(!this.is_has_falling_block()){
            if (Input.GetMouseButtonDown(0))
            {
                foreach(BlockControl block in this.blocks)//���콺 ��ư�� ���ȴٸ� blocks �迭�� ��� ��Ҹ� ���ʷ� ó��
                {
                    if(!block.isGrabbable())
                    {
                        continue;//����� ���� �� ���ٸ� ������ ó������ ����
                    }
                    if (!block.isContainedPosition(mouse_position_xy))
                    {
                        continue;//���콺 ��ġ�� ��� ���� ���ΰ� �ƴ϶�� ������ ó������ ����
                    }
                    this.grabbed_block = block;//ó������ ����� grabbed_block�� ���
                    this.grabbed_block.beginGrab();//����� ���� ó�� ����
                    break;
                }
            }
            //}
        }
        else
        {
            //����� ����� ��
            if(!Input.GetMouseButton(0))
            {
                this.grabbed_block.endGrab();//���콺 ��ư�� ������ ���� ������ ����� ������ ���� ó���� ���� �� grabbed_block�� ��쵵�� �Ѵ�.
                this.grabbed_block = null;
            }
        }
    }

    public bool unprojectMousePosition(out Vector3 world_position, Vector3 mouse_position)//���콺�� ���� ��� ��� ǥ���� ����Ű�� �� ���.
     //9x9 ��� ǥ�鿡 ������ ���� �ΰ�, ī�޶󿡼� ���콺 ��ǥ�� ���� ���� ������� ���� ���� ���ο� ���� ���콺�� ����Ű�� ���� 3���� ������ ��ġ�� �� �� �ִ�.
    {// *  out = �ʱ�ȭ ���� ���� ������ ���������� ���� ����(ref�� �ʱ�ȭ �� ������ ����.)
        bool ret;
        Plane plane = new Plane(Vector3.back, new Vector3(0.0f, 0.0f, -Block.COLLISION_SIZE / 2.0f));//ī�޶� ���ؼ� �ڸ� ���ϰ�, ����� ���� ũ�⸸ŭ �տ� ��ġ�� ��.

        Ray ray = this.main_camera.GetComponent<Camera>().ScreenPointToRay(mouse_position);//ī�޶�� ���콺�� ����ϴ� ���� �����.
        float depth;//������ �ǿ� ����� �� ������ ��ϵǴ� ����

        if(plane.Raycast(ray, out depth))//������ �ǿ� ����� ��
        {
            world_position = ray.origin + ray.direction * depth;//�μ� world_position�� ���콺 ��ġ��.
            ret = true;
        }
        else//���� �ʾҴٸ� ������������ 0�� ���ͷ�.
        {
            world_position = Vector3.zero;
            ret= false;
        }
        return (ret);
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
