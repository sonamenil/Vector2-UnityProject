using System.Collections.Generic;
using Nekki.Vector.Core.Models;
using Nekki.Vector.Core.Node;
using Nekki.Vector.Core.Result;
using Nekki.Vector.Core.Runners;

namespace Nekki.Vector.Core.Controllers
{
	public class ControllerCollision
	{
		private Model _Model;

		private ModelObject _ModelObject;

		private List<ModelNode> _CollisionNodes;

		private List<ModelLine> _CollisionEdges;

		private ControllerPlatform _ControllerPlatform;

		private ControllerArea _ControllerArea;

		public ControllerPlatform ControllerPlatform
		{
			get
			{
				return _ControllerPlatform;
			}
		}

		public ControllerArea ControllerArea
		{
			get
			{
				return _ControllerArea;
			}
		}

		public bool IsPlay
		{
			get
			{
				return _Model.Type == ModelType.Human && ((ModelHuman)_Model).IsPlay;
			}
		}

		public List<AreaRunner> ActiveAreas
		{
			get
			{
				return _ControllerArea.ActiveAreas;
			}
		}

		public ControllerCollision(Model p_model)
		{
			_Model = p_model;
			_ModelObject = p_model.ModelObject;
			_CollisionEdges = _ModelObject.CollisibleEdges;
			_CollisionNodes = _ModelObject.CollisibleNodes;
			if (p_model.Type == ModelType.Human)
			{
				ModelHuman modelHuman = (ModelHuman)p_model;
				_ControllerPlatform = new ControllerPlatform(modelHuman);
				_ControllerArea = new ControllerArea(modelHuman);
			}
		}

		public void Reset()
		{
			_ControllerPlatform.Reset();
			_ControllerArea.Reset();
		}

		public void UpdateQuad(QuadRunner p_quad)
		{
			p_quad.IsRender = false;
			if (!IsCollide(p_quad.Rectangle) || !p_quad.IsEnabled)
			{
				return;
			}
			ModelHuman modelHuman = (ModelHuman)_Model;
			switch (p_quad.TypeClass)
			{
			case TypeRunner.Trigger:
				modelHuman.ControllerTrigger.Check((TriggerRunner)p_quad);
				return;
			case TypeRunner.Area:
				_ControllerArea.Check((AreaRunner)p_quad);
				return;
			}
			if (modelHuman.IsPlay)
			{
				_ControllerPlatform.Render(p_quad);
				DispatchCollision(p_quad);
			}
			else
			{
				PushingNodes(p_quad);
			}
		}

		public void UpdatePlatform(QuadRunner p_platform)
		{
			if (IsCollide(p_platform.Rectangle))
			{
				if (IsPlay)
				{
					_ControllerPlatform.Render(p_platform);
					DispatchCollision(p_platform);
				}
				else
				{
					PushingNodes(p_platform);
				}
			}
		}

		public Collision CrossModel(Model p_model, Vector3f p_vector1, Vector3f p_vector2)
		{
			if (p_model == null || !p_model.IsEnabled)
			{
				return null;
			}
			foreach (ModelLine item in p_model.ModelObject.EdgesAll)
			{
				Vector3f vector3f = Vector3f.Cross(p_vector1, p_vector2, item.Start.Start, item.End.Start);
				if (vector3f == null)
				{
					continue;
				}
				Collision collision = new Collision();
				collision.Point = vector3f;
				collision.Edge = item;
				return collision;
			}
			return null;
		}

		public void DispatchCollision(QuadRunner p_platform)
		{
			ModelHuman modelHuman = _Model as ModelHuman;
			if (modelHuman == null || !IsCollide(p_platform))
			{
				return;
			}
			ModelLine modelLine = null;
			for (int i = 0; i < _CollisionEdges.Count; i++)
			{
				modelLine = _CollisionEdges[i];
				List<Cross> list = p_platform.CrossByEdge(modelLine.Start.Start, modelLine.End.Start);
				if (list.Count != 0)
				{
					Collision collision = new Collision();
					collision.Model = _Model;
					collision.Edge = modelLine;
					collision.Point = list[0].Point;
					collision.Platform = p_platform;
					Collision p_collision = collision;
					modelHuman.OnCollisionPlatform(p_collision);
					break;
				}
			}
		}

		public void PushingNodes(QuadRunner p_platform)
		{
			foreach (ModelNode collisionNode in _CollisionNodes)
			{
				Vector3fLine vector3fLine = p_platform.Friction(collisionNode.End, collisionNode.Start);
				if (vector3fLine != null)
				{
					vector3fLine.Start.Z = collisionNode.Start.Z;
					vector3fLine.End.Z = collisionNode.End.Z;
					collisionNode.PositionStart(vector3fLine.Start);
					collisionNode.PositionEnd(vector3fLine.End);
				}
			}
		}

		public bool IsCollide(QuadRunner p_quad)
		{
			return ((ModelHuman)_Model).CollisionBox.Intersect(p_quad.Rectangle);
		}

		public bool IsCollide(Rectangle p_rectangle)
		{
			return p_rectangle.Intersect(_Model.Rectangle);
		}
	}
}
