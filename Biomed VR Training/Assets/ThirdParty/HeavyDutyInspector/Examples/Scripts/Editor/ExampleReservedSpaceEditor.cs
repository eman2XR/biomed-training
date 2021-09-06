//----------------------------------------------
//            Heavy-Duty Inspector
//         Copyright © 2017  Illogika
//----------------------------------------------
using UnityEngine;
using UnityEditor;
using HeavyDutyInspector;

[CustomEditor(typeof(ExampleReservedSpace))]
public class ExampleReservedSpaceEditor : Editor
{
	private Vector2 scrollPosition;
	private Rect scrollRect;

	public ExampleReservedSpaceEditor()
	{
		scrollRect = new Rect(0, 0, 500, 500);
	}

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		Rect scrollViewRect = ReservedSpaceDrawer.spaceRects[0];
		scrollViewRect.height -= 4;
		scrollViewRect.y += 2;

		scrollPosition = GUI.BeginScrollView(scrollViewRect, scrollPosition, scrollRect);
		{
			GUI.TextArea(scrollRect, "Lorem ipsum dolor sit amet, consectetur adipiscing elit. In tincidunt tempus felis nec efficitur. Integer diam ante, venenatis eget vulputate nec, lobortis vel orci. Duis luctus velit massa, vitae suscipit orci venenatis nec. Suspendisse eu mi vitae lacus lobortis imperdiet ac non ligula. Nulla facilisi. Mauris congue metus lacus, et iaculis eros pharetra eu. Proin scelerisque quam non vehicula tincidunt. Ut tincidunt dapibus odio, nec consectetur elit tempor non. Donec tristique risus a sem ullamcorper maximus. Proin maximus, magna ultricies mollis ullamcorper, metus lacus pulvinar neque, eu accumsan tellus magna vel ipsum. Pellentesque nulla leo, cursus a sem sit amet, finibus luctus neque.\n\nProin et accumsan massa.Praesent vel tincidunt mauris.Vivamus gravida eu mauris nec aliquet.Nunc fringilla ipsum quis erat varius fringilla vitae ac odio.Maecenas vitae tempor tellus.Duis quis finibus nibh.Praesent aliquet eros quis enim vulputate, luctus sagittis lacus ultrices.Vestibulum a bibendum nunc.\n\nLorem ipsum dolor sit amet, consectetur adipiscing elit.Phasellus egestas convallis elementum.Aenean sollicitudin aliquet nulla eu finibus.Proin luctus diam et ligula convallis maximus.Aenean vehicula mi quis ante sodales rutrum.Pellentesque pretium et massa sit amet faucibus.Suspendisse et posuere leo.Mauris consectetur, metus at finibus sodales, arcu libero tincidunt quam, vitae sodales lacus sem viverra magna.Quisque scelerisque erat et tristique sagittis.In nec tristique augue.Suspendisse lobortis lobortis quam, porta aliquet massa faucibus quis.Donec justo dui, aliquet lacinia sapien quis, consequat laoreet orci.Phasellus nec nunc dui.Curabitur fermentum ac lectus non malesuada.Phasellus cursus eu neque consequat sodales.\n\nProin a augue tincidunt, molestie metus id, tincidunt metus.Vestibulum eu lorem sed augue volutpat cursus.Integer eu turpis nec mauris tempor rhoncus maximus vitae turpis.Donec luctus nisl est, venenatis placerat dui ornare vitae.Vivamus quis pulvinar massa.Vestibulum at semper lorem.Fusce risus odio, lobortis vel euismod vel, egestas nec mauris.Sed congue, justo sed suscipit eleifend, libero nisi molestie nulla, non efficitur turpis dolor eu massa.\n\nVivamus bibendum hendrerit mauris, sed tristique erat congue vitae.Vivamus at mauris a est suscipit placerat ut ac orci.Vivamus vel nibh sed lectus mattis accumsan.Aliquam non venenatis ipsum.Lorem ipsum dolor sit amet, consectetur adipiscing elit.Nullam eget nisl justo.Donec condimentum porta tellus, vitae cursus tortor vestibulum eget.Mauris non metus feugiat, dictum nibh at, blandit erat.Aenean nunc urna, molestie at finibus in, sodales et libero.Donec ac nunc ipsum.Nullam pellentesque augue id mauris tincidunt, quis congue ligula egestas.");
		}
		GUI.EndScrollView();
	}
}
