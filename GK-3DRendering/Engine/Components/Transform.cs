using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace GK_3DRendering.Engine.Components
{
    class Transform
    {
        [InspectorIgnore]
        public Vector3 Position 
        { 
            get
            {
                return TransformMatrix.ExtractTranslation();
            }
        }
        [InspectorIgnore]
        public Vector3 Scale { get; private set; }

        
        public Vector3 LocalPosition 
        { 
            get
            {
                UpdateTransform();
                return _localPosition;
            }
            set
            {
                SetPosition(value);
            }
        }
        public Vector3 LocalRotationDeg
        {
            get
            {
                Vector3 rotationDeg = LocalRotation;
                rotationDeg.X = MathHelper.RadiansToDegrees(rotationDeg.X);
                rotationDeg.Y = MathHelper.RadiansToDegrees(rotationDeg.Y);
                rotationDeg.Z = MathHelper.RadiansToDegrees(rotationDeg.Z);
                return rotationDeg;
            }
            set
            {
                SetRotationDeg(value);
            }
        }
        [InspectorIgnore]
        public Vector3 LocalRotation
        {
            get
            {
                UpdateTransform();
                return _localRotation;
            }
            set
            {
                SetRotation(value);
            }
        }
        public Vector3 LocalScale
        {
            get
            {
                UpdateTransform();
                return _localScale;
            }
            set
            {
                SetScale(value);
            }
        }

        [InspectorIgnore]
        public Transform? Parent = null;
        [InspectorIgnore]
        public List<Transform> Children = new();

        private bool _isDirty = true;
        private bool _isDirtyLocal = true;

        private Matrix4 _localTransformMatrix;
        private Matrix4 _localRotationMatrix;
        private Matrix4 _transformMatrix;
        private Matrix4 _rotationMatrix;

        private Vector3 _localPosition;
        private Vector3 _localRotation;
        private Vector3 _localScale;

        [InspectorIgnore]
        public Matrix4 TransformMatrix
        {
            get
            {
                UpdateTransform();
                return _transformMatrix;
            }
        }

        [InspectorIgnore]
        public Matrix4 RotationMatrix
        {
            get
            {
                UpdateTransform();
                return _rotationMatrix;
            }

        }

        [InspectorIgnore]
        public Matrix4 LocalTransformMatrix
        {
            get
            {
                UpdateTransform();
                return _localTransformMatrix;
            }
        }

        [InspectorIgnore]
        public Matrix4 LocalRotationMatrix
        {
            get
            {
                UpdateTransform();
                return _localRotationMatrix;
            }

        }

        

        [InspectorIgnore]
        public Vector3 Forward
        {
            get
            {
                return new Vector3(Vector4.TransformRow(new Vector4(-Vector3.UnitZ, 1.0f), RotationMatrix)).Normalized();
            }
        }

        [InspectorIgnore]
        public Vector3 Up
        {
            get
            {
                return new Vector3(Vector4.TransformRow(new Vector4(Vector3.UnitY, 1.0f), RotationMatrix)).Normalized();
            }
        }

        [InspectorIgnore]
        public Vector3 Right
        {
            get
            {
                return new Vector3(Vector4.TransformRow(new Vector4(Vector3.UnitX, 1.0f), RotationMatrix)).Normalized();
            }
        }

        [InspectorIgnore]
        public Vector3 LocalForward
        {
            get
            {
                return new Vector3(Vector4.TransformRow(new Vector4(-Vector3.UnitZ, 1.0f), LocalRotationMatrix)).Normalized();
            }
        }

        [InspectorIgnore]
        public Vector3 LocalUp
        {
            get
            {
                return new Vector3(Vector4.TransformRow(new Vector4(Vector3.UnitY, 1.0f), LocalRotationMatrix)).Normalized();
            }
        }

        [InspectorIgnore]
        public Vector3 LocalRight
        {
            get
            {
                return new Vector3(Vector4.TransformRow(new Vector4(Vector3.UnitX, 1.0f), LocalRotationMatrix)).Normalized();
            }
        }


        public Transform(Vector3 position, Vector3 rotation, Vector3 scale)
        {
            LocalPosition = position;
            LocalRotation = rotation;
            LocalScale = scale;
        }

        public Transform(float posX, float posY, float posZ, float rotX, float rotY, float rotZ, float scaleX, float scaleY, float scaleZ) :
            this(new Vector3(posX, posY, posZ), new Vector3(rotX, rotY, rotZ), new Vector3(scaleX, scaleY, scaleZ))
        { }

        public Transform(Vector3 position) : this(position, Vector3.Zero, Vector3.One)
        { }

        public Transform(Vector3 position, Vector3 rotation) : this(position,rotation,Vector3.One)
        { }

        public Transform() : this(Vector3.Zero, Vector3.Zero, Vector3.One)
        { }

        private void CalculateLocalTransformMatrix()
        {

            Matrix4.CreateTranslation(_localPosition, out Matrix4 translation);
            CalculateLocalRotationMatrix();
            Matrix4.CreateScale(_localScale, out Matrix4 scale);

            _localTransformMatrix = scale * _localRotationMatrix * translation;
        }

        private void CalculateLocalRotationMatrix()
        {
            Matrix4.CreateRotationY(_localRotation.Y, out Matrix4 rotationY);
            Matrix4.CreateRotationX(_localRotation.X, out Matrix4 rotationX);
            Matrix4.CreateRotationZ(_localRotation.Z, out Matrix4 rotationZ);

            _localRotationMatrix = rotationZ * rotationX * rotationY;
        }

        private void CalculateTransformMatrix()
        {
            CalculateLocalTransformMatrix();
            Matrix4 parentTransform = Matrix4.Identity;
            Matrix4 parentRotation = Matrix4.Identity;
            if(Parent != null)
            {
                parentTransform = Parent.TransformMatrix;
                parentRotation = Parent.RotationMatrix;
            }
            _transformMatrix =  _localTransformMatrix * parentTransform ;
            _rotationMatrix =  _localRotationMatrix * parentRotation ;
        }

        public void SetPosition(Vector3 position)
        {
            _localPosition = position;
            SetDirty();
        }

        public void Translate(Vector3 translation)
        {
            _localPosition += translation;
            SetDirty();
        }

        public void SetRotation(Vector3 rotation)
        {
            _localRotation = rotation;
            KeepRoatationInRange();
            SetDirty();
        }

        public void Rotate(Vector3 rotation)
        {
            _localRotation += rotation;
            KeepRoatationInRange();
            SetDirty();
        }

        public void SetRotationDeg(Vector3 rotation)
        {
            rotation.X = MathHelper.DegreesToRadians(rotation.X);
            rotation.Y = MathHelper.DegreesToRadians(rotation.Y);
            rotation.Z = MathHelper.DegreesToRadians(rotation.Z);
            SetRotation(rotation);
            SetDirty();
        }

        public void RoatateDeg(Vector3 rotation)
        {
            rotation.X = MathHelper.DegreesToRadians(rotation.X);
            rotation.Y = MathHelper.DegreesToRadians(rotation.Y);
            rotation.Z = MathHelper.DegreesToRadians(rotation.Z);
            Rotate(rotation);
            SetDirty();
        }

        public void KeepRoatationInRange()
        {
            _localRotation = new Vector3(KeepRadiansInRange(_localRotation.X),
                                        KeepRadiansInRange(_localRotation.Y),
                                        KeepRadiansInRange(_localRotation.Z));
        }

        public float KeepRadiansInRange(float theta)
        {
            theta = theta % (2 * (float)Math.PI); 

            if (theta > (float)Math.PI) 
            {
                theta -= 2 * (float)Math.PI;
            }
            else if (theta < -(float)Math.PI) 
            {
                theta += 2 * (float)Math.PI;
            }

            return theta;
        }

        public void SetScale(Vector3 scale)
        {
            _localScale = scale;
            SetDirty();
        }

        public void SetDirty()
        {
            _isDirty = true;
            foreach (var child in Children)
            {
                child.SetDirty();
            }
        }

        public void AddChild(Transform child)
        {
            child.SetParent(this);
        }

        public void SetParent(Transform parent)
        {
            if(Parent != null)
            {
                Parent.Children.Remove(this);
            }

            Parent = parent;
            Parent.Children.Add(this);
            SetDirty();
        }

        private void UpdateTransform()
        {
            if (_isDirty)
            {
                CalculateTransformMatrix();
                _isDirty = false;
            }
        }

        public void LookAtTarget(Vector3 targetPosition)
        {
            Vector3 direction = targetPosition - Position;

            direction = Vector3.TransformRow(direction, new Matrix3(RotationMatrix) * new Matrix3(LocalRotationMatrix.Inverted()));

            direction.Normalize();

            float pitch = MathF.Asin(direction.Y); 

            float yaw = MathF.Atan2(-direction.X, -direction.Z);

            SetRotation(new Vector3(pitch, yaw, 0));
        }

    }
}
