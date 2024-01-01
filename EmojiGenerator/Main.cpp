# include <Siv3D.hpp> // Siv3D v0.6.12

namespace
{
	constexpr bool isSaveImage = true;
	constexpr int commonImageSize = 64;
	constexpr auto solutionRoot = U"../../"_sv;
	constexpr auto skinOutputDirectory = U"../../EmojiInput/Resource/"_sv;
	constexpr auto imageOutputDirectory = U"../../EmojiInput/Resource/emoji_icon/aliased/"_sv;

	struct EmojiData
	{
		String emoji;
		String aliases;
		bool skin;
	};

	void saveEmojiImage(StringView outputDirectory, const String& filename, const String& emoji)
	{
		auto image = Image(Emoji(emoji)).squareClipped();
		image.scale(Size::One() * commonImageSize);

		if (image.size().minComponent() == 0)
		{
			Console.writeln(U"Invalid image: " + filename);
			return;
		}

		if constexpr (not isSaveImage) return;

		if (image.save(outputDirectory + filename))
		{
			// Console.writeln(U"Saved: " + filename);
		}
		else
		{
			Console.writeln(U"Failed to save: " + filename);
		}
	}

	String getSkinEmoji(const EmojiData& e, char32_t skin)
	{
		String skinEmoji;
		constexpr char32_t zwj = 0x200d;
		const auto zwjIndex1 = e.emoji.indexOf(zwj);
		const auto zwjIndex2 = e.emoji.indexOf(zwj, zwjIndex1 + 1);
		if (zwjIndex1 == String::npos)
		{
			skinEmoji = e.emoji + skin;
		}
		else
		{
			skinEmoji = String(e.emoji).insert(zwjIndex1, String().append(skin));
			if (zwjIndex2 != String::npos)
			{
				// 👩🏼‍🤝‍👨🏽 など2色の肌色
				skinEmoji += skin;
			}
		}
		return skinEmoji;
	}

	String getSkinMap(const StringView skinColors)
	{
		String skinMetadata{};
		for (auto&& skin : skinColors)
		{
			if (not skinMetadata.empty()) skinMetadata += U" ";
			skinMetadata += U"{:X}"_fmt(static_cast<uint32>(skin));
		}
		return skinMetadata;
	}

	String saveSkinEmojis(const StringView skinColors, const Array<EmojiData>::value_type& e)
	{
		String concatenatedSkinEmoji{};
		for (auto&& skin : skinColors)
		{
			const auto filename = e.aliases + U"_{:X}"_fmt(static_cast<uint32>(skin)) + U".png";
			String skinEmoji = getSkinEmoji(e, skin);
			if (not concatenatedSkinEmoji.empty()) concatenatedSkinEmoji += U" "_sv;
			concatenatedSkinEmoji += skinEmoji;
			saveEmojiImage(imageOutputDirectory, filename, skinEmoji);
		}
		return concatenatedSkinEmoji;
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

	Array<JSON> skinJsonArray{};
	constexpr StringView skinColors = U"🏻🏼🏽🏾🏿";

	// 絵文字を画像にして保存していく
	for (int i = 0; i < emojis.size(); ++i)
	{
		const auto& e = emojis[i];
		JSON jsonElement{};
		saveEmojiImage(imageOutputDirectory, e.aliases + U".png", e.emoji);
		if (e.skin)
		{
			// 肌色変更可能の絵文字
			const String concatenatedSkinEmoji = saveSkinEmojis(skinColors, e);
			jsonElement[U"key"] = e.aliases;
			jsonElement[U"emojis"] = concatenatedSkinEmoji;
			skinJsonArray.push_back(jsonElement);
		}
	}

	Console.writeln(U"Completed save emojis");

	JSON skinJson{};
	skinJson[U"map"] = getSkinMap(skinColors);
	skinJson[U"skins"] = skinJsonArray;
	skinJson.save(skinOutputDirectory + U"emoji_skin.json");

	Console.writeln(U"Saved skin data");

	System::MessageBoxOK(U"Process completed", MessageBoxStyle::Default);
}
