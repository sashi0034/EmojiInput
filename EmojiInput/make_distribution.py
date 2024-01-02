import os
import shutil
import zipfile
import glob

target_dir = "bin/Release/EmojiInput"

source_root_dir = "bin/Release/net7.0-windows"

source_content_files = [
    "EmojiInput.deps.json",
    "EmojiInput.runtimeconfig.json",
]

source_content_dirs = [
    "Resource",
]


def copy_source_files(source_dir, target_dir):
    for file in glob.glob(os.path.join(source_dir, '*.exe')):
        dest_file = os.path.join(target_dir, os.path.basename(file))
        shutil.copy(file, dest_file)
    for file in glob.glob(os.path.join(source_dir, '*.dll')):
        dest_file = os.path.join(target_dir, os.path.basename(file))
        shutil.copy(file, dest_file)
    for file in source_content_files:
        dest_file = os.path.join(target_dir, os.path.basename(file))
        shutil.copy(source_root_dir + "/" + file, dest_file)


def main():
    if os.path.exists(target_dir):
        shutil.rmtree(target_dir)

    os.makedirs(target_dir)

    for folder in source_content_dirs:
        if os.path.exists(source_root_dir + "/" + folder):
            shutil.copytree(source_root_dir + "/" + folder, os.path.join(target_dir, folder))

    copy_source_files(source_root_dir, target_dir)

    with zipfile.ZipFile(target_dir + ".zip", 'w', compression=zipfile.ZIP_DEFLATED) as zipf:
        for foldername, subfolders, filenames in os.walk(target_dir):
            for filename in filenames:
                file_path = os.path.join(foldername, filename)
                zipf.write(file_path, os.path.relpath(file_path, os.path.dirname(target_dir)))


if __name__ == "__main__":
    main()
