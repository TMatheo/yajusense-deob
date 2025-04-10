import os
import subprocess

def run_code2prompt(template_file):
    """指定されたテンプレートファイルとハードコードされたインクルード/除外パターンでcode2promptを実行する関数"""
    include_pattern = "*.cs"
    exclude_pattern = "Dusk.AssemblyInfo.cs,MyPluginInfo.cs"
    command = [
        "code2prompt",
        ".",
        "--template",
        template_file,
        "--encoding=r50k_base",
        "--include",
        include_pattern,
        "--exclude",
        exclude_pattern
    ]
    try:
        subprocess.run(command, check=True)
        print(f"code2prompt がテンプレートファイル '{template_file}'、インクルードパターン '{include_pattern}'、除外パターン '{exclude_pattern}' で実行されました。")
    except subprocess.CalledProcessError as e:
        print(f"エラー: code2prompt の実行に失敗しました: {e}")
    except FileNotFoundError:
        print("エラー: 'code2prompt' コマンドが見つかりませんでした。インストールされているか、システムのPATHに含まれているか確認してください。")

if __name__ == "__main__":
    hbs_dir = "hbs"  # hbsサブディレクトリを指定
    if not os.path.exists(hbs_dir):
        print(f"エラー: '{hbs_dir}' ディレクトリが見つかりませんでした。")
        exit(1)
    
    hbs_files = [f for f in os.listdir(hbs_dir) if f.endswith('.hbs')]

    if not hbs_files:
        print(f"'{hbs_dir}' ディレクトリに .hbs ファイルが見つかりませんでした。")
    else:
        print("利用可能な .hbs ファイル:")
        for i, filename in enumerate(hbs_files):
            print(f"{i + 1}. {filename}")

        while True:
            try:
                selection = int(input("使用するテンプレートファイルの番号を選択してください: "))
                if 1 <= selection <= len(hbs_files):
                    selected_template = os.path.join(hbs_dir, hbs_files[selection - 1])
                    break
                else:
                    print("無効な選択です。リストにある番号を入力してください。")
            except ValueError:
                print("無効な入力です。数値を入力してください。")

        run_code2prompt(selected_template)
