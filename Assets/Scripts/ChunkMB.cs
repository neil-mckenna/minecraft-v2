using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkMB: MonoBehaviour
{
	Chunk owner;
	public ChunkMB(){}
	public void SetOwner(Chunk o)
	{
		owner = o;
		InvokeRepeating("SaveProgress",10,1000);

	}

	public IEnumerator HealBlock(Vector3 bpos)
	{
		yield return new WaitForSeconds(3);
		int x = (int) bpos.x;
		int y = (int) bpos.y;
		int z = (int) bpos.z;

		if(owner.chunkData[x,y,z].bType != Block.BlockType.AIR)
			owner.chunkData[x,y,z].Reset();
	}

	public IEnumerator Drop(Block b, Block.BlockType bt, int maxdrop)
	{
		Block thisBlock = b;
		Block prevBlock = null;
		for(int i = 0; i < maxdrop; i++)
		{
			Block.BlockType previousType = thisBlock.bType;
			if(previousType != bt)
				thisBlock.SetType(bt);
			if(prevBlock != null)
				prevBlock.SetType(previousType);

			prevBlock = thisBlock;
			b.owner.Redraw();
			
			yield return new WaitForSeconds(0.2f);
			Vector3 pos = thisBlock.position;
			
			thisBlock = thisBlock.GetBlock((int)pos.x,(int)pos.y-1,(int)pos.z);
			if(thisBlock.isSolid)
			{	
				yield break;
			}
		}
	}

	public IEnumerator Flow(Block b, Block.BlockType bt, int strength, int maxsize)
	{
		//reduce the strenth of the fluid block
		//with each new block created
		if(maxsize <= 0) yield break;
		if(b == null) yield break;
		if(strength <= 0) yield break;
		if(b.bType != Block.BlockType.AIR) yield break;
		b.SetType(bt);
		b.currentHealth = strength;
		b.owner.Redraw();
		yield return new WaitForSeconds(1);

		int x = (int) b.position.x;
		int y = (int) b.position.y;
		int z = (int) b.position.z;

		//flow down if air block beneath
		Block below = b.GetBlock(x,y-1,z);
		if(below != null && below.bType == Block.BlockType.AIR)
		{
			StartCoroutine(Flow(b.GetBlock(x,y-1,z),bt,strength,--maxsize));
			yield break;
		}
		else //flow outward
		{
			--strength;
			--maxsize;
			//flow left
			World.queue.Run(Flow(b.GetBlock(x-1,y,z),bt,strength,maxsize));
			yield return new WaitForSeconds(1);

			//flow right
			World.queue.Run(Flow(b.GetBlock(x+1,y,z),bt,strength,maxsize));
			yield return new WaitForSeconds(1);

			//flow forward
			World.queue.Run(Flow(b.GetBlock(x,y,z+1),bt,strength,maxsize));
			yield return new WaitForSeconds(1);

			//flow back
			World.queue.Run(Flow(b.GetBlock(x,y,z-1),bt,strength,maxsize));
			yield return new WaitForSeconds(1);

			// flow down left for
			World.queue.Run(Flow(b.GetBlock(x-1,y -1,z + 1),bt,strength, maxsize));
			yield return new WaitForSeconds(1);

			// flow down left back
			World.queue.Run(Flow(b.GetBlock(x-1,y -1,z - 1),bt,strength, maxsize));
			yield return new WaitForSeconds(1);

			// flow down right for
			World.queue.Run(Flow(b.GetBlock(x + 1,y -1,z + 1),bt,strength, maxsize));
			yield return new WaitForSeconds(1);

			// flow down right back
			World.queue.Run(Flow(b.GetBlock(x+1,y -1,z -1),bt,strength, maxsize));
			yield return new WaitForSeconds(1);
		}


		
	}

	void SaveProgress()
	{
		if(owner.changed)
		{
			owner.Save();
			owner.changed = false;
		}
	}
}
