using System.Collections;

namespace SR2E.Commands;

public class FloatCommand : SR2CCommand
{
    public override string ID => "floaty";
    public override string Usage => "floaty <duration>";

    public override string Description =>
        "Temporarily disables gravity for the selected object for the specified duration.";

    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0)
        {
            var list = new List<string>();
            list.Add("10");
            list.Add("20");
            list.Add("50");
            list.Add("100");
            return list;
        }

        return null;
    }

    public override bool Execute(string[] args)
    {
        if (Physics.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), out var hit))
        {
            if (hit.rigidbody == null)
            {
                SR2Console.SendError($"Object {hit} has no rigidbody");
                return false;
            }

            MelonCoroutines.Start(TimeGravity(hit, args[0]));
        }
        else
        {
            SR2Console.SendWarning("Not looking at a valid object!");
            return false;
        }
        return true;
    }

    private IEnumerator TimeGravity(RaycastHit hit, string duration)
    {
        hit.rigidbody.useGravity = false;
        yield return new WaitForSecondsRealtime(float.Parse(duration));
        hit.rigidbody.useGravity = true;
    }
}