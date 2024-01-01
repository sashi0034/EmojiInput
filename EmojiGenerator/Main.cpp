# include <Siv3D.hpp> // Siv3D v0.6.12

void Main()
{
	Texture emoji{U"😀"_emoji};

	while (System::Update())
	{
		emoji.draw();
	}
}
