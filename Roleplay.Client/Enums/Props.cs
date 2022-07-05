using CitizenFX.Core;
using CitizenFX.Core.Native;
using Roleplay.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roleplay.Client.Enums
{
    internal class Props
    {
        public static void CreateProp(string propName, Vector3 position, Vector3 rotation)
        {
            try
            {
                IEnumerable<string> source1 = ((IEnumerable<string>)Enum.GetNames(typeof(ObjectHash))).Where<string>((Func<string, bool>)(n => n.Contains(propName)));
                if (!(source1 is IList<string> stringList))
                    stringList = (IList<string>)source1.ToList<string>();
                IList<string> source2 = stringList;
                if (!source2.Any<string>())
                    return;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public static List<int> FindProps(string propName, float distanceThreshold)
        {
            try
            {
                int hash = Game.GenerateHash(propName);
                int num1 = 0;
                int firstObject = API.FindFirstObject(ref num1);
                List<int> intList = new List<int>();
                Vector3 position = ((Game.PlayerPed).Position);
                while (API.FindNextObject(firstObject, ref num1))
                {
                    int num2 = num1;
                    int entityModel = API.GetEntityModel(num2);
                    if (entityModel == hash || entityModel == 2121050683 || entityModel == -63539571)
                    {
                        Entity entity = Entity.FromHandle(num2);
                        if (((PoolObject)entity).Exists() && (double)((Vector3)position).DistanceToSquared2D(entity.Position) <= (double)distanceThreshold)
                            intList.Add(num2);
                    }
                }
                API.EndFindObject(firstObject);
                return intList;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return new List<int>();
            }
        }

        public static List<int> FindProps(float distanceThreshold)
        {
            try
            {
                int num1 = 0;
                int firstObject = API.FindFirstObject(ref num1);
                List<int> intList = new List<int>();
                Vector3 position = ((Game.PlayerPed).Position);
                while (API.FindNextObject(firstObject, ref num1))
                {
                    int num2 = num1;
                    switch (API.GetEntityModel(num2))
                    {
                        case -63539571:
                        case 2121050683:
                            continue;
                        default:
                            Entity entity = Entity.FromHandle(num2);
                            if (((PoolObject)entity).Exists() && (double)((Vector3)position).DistanceToSquared2D(entity.Position) <= (double)distanceThreshold)
                            {
                                intList.Add(num2);
                                continue;
                            }
                            continue;
                    }
                }
                API.EndFindObject(firstObject);
                return intList;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return new List<int>();
            }
        }

        public static List<int> FindProps3D(
        string propName,
        Vector3 searchFromPos,
        float distanceThreshold)
        {
            try
            {
                int hash = Game.GenerateHash(propName);
                int num1 = 0;
                int firstObject = API.FindFirstObject(ref num1);
                List<int> intList = new List<int>();
                while (API.FindNextObject(firstObject, ref num1))
                {
                    int num2 = num1;
                    int entityModel = API.GetEntityModel(num2);
                    if (entityModel == hash || entityModel == 2121050683 || entityModel == -63539571)
                    {
                        Entity entity = Entity.FromHandle(num2);
                        if (((PoolObject)entity).Exists() && (double)((Vector3)searchFromPos).DistanceToSquared(entity.Position) <= (double)distanceThreshold)
                            intList.Add(num2);
                    }
                }
                API.EndFindObject(firstObject);
                return intList;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return new List<int>();
            }
        }

    }
}
