# include <Siv3D.hpp> // Siv3D v0.6.12

namespace
{
	struct EmojiData
	{
		String emoji;
		String aliases;
		bool skin;
	};

	constexpr bool isSaveImage = true;
	constexpr int commonImageSize = 64;
	constexpr auto solutionRoot = U"../../"_sv;
	constexpr auto outputDirectory = U"../../EmojiInput/Resource/emoji_icon/aliased/"_sv;
}

void saveEmojiImage(StringView outputDirectory, const String& filename, const String& emoji)
{
	auto image = Image(Emoji(emoji));
	image.scale(((SizeF(image.size()) / image.size().maxComponent()) * commonImageSize).asPoint());;

	if (image.size().minComponent() == 0)
	{
		Console.writeln(U"Invalid image: " + filename);
		return;
	}

	if constexpr (not isSaveImage) return;

	image.resize(Size::One() * commonImageSize);

	if (image.save(outputDirectory + filename))
	{
		// Console.writeln(U"Saved: " + filename);
	}
	else
	{
		Console.writeln(U"Failed to save: " + filename);
	}
}

void Main()
{
	const JSON json = JSON::Load(solutionRoot + U"EmojiInput/Resource/emoji.json");

	if (not json)
	{
		System::MessageBoxOK(U"Missing json file", MessageBoxStyle::Error);
		return;
	}

	Array<EmojiData> emojis{};

	// JSONから絵文字データを収集
	for (auto j : json.arrayView())
	{
		emojis.push_back({
			j[U"emoji"].getString(),
			j[U"aliases"].arrayView()[0].getString(),
			j[U"skin_tones"].getOr<bool>(false)
		});
	}

	Console.writeln(U"Found {} emojis"_fmt(emojis.size()));

	if (emojis.size() != emojis.map([](auto&& e) { return e.aliases; }).sort_and_unique().size())
	{
		System::MessageBoxOK(U"Duplicated aliases found", MessageBoxStyle::Error);
		return;
	}

	// 絵文字を画像にして保存していく
	for (int i = 0; i < emojis.size(); ++i)
	{
		const auto& e = emojis[i];
		if (e.skin)
		{
			constexpr StringView skinColors = U"🏻🏼🏽🏾🏿";
			for (const auto skin : skinColors)
			{
				const auto filename = e.aliases + U"_{:X}"_fmt(static_cast<uint32>(skin)) + U".png";
				constexpr char32_t zwj = 0x200d;
				const int zwjIndex = e.emoji.indexOf(zwj);
				const auto skinEmoji = zwjIndex == e.emoji.npos
					                       ? e.emoji + skin
					                       : String(e.emoji).insert(zwjIndex, String().append(skin));
				saveEmojiImage(outputDirectory, filename, skinEmoji);
			}
		}
		else
		{
			saveEmojiImage(outputDirectory, e.aliases + U".png", e.emoji);
		}
	}

	System::MessageBoxOK(U"Process completed", MessageBoxStyle::Default);
}
