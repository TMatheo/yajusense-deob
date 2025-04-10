import os
import shutil

def overwrite_dlls_from_directory(source_dir):
    """
    指定したディレクトリから同名のDLLを検索し、現在のディレクトリにあるDLLを上書きコピーする
    
    Args:
        source_dir (str): DLLファイルが格納されているソースディレクトリへのパス
    """
    # 現在の作業ディレクトリを取得
    current_dir = os.getcwd()
    
    # ソースディレクトリ内のすべてのDLLファイルをリストアップ
    source_files = [f for f in os.listdir(source_dir) if f.lower().endswith('.dll')]
    
    if not source_files:
        print(f"ソースディレクトリにDLLファイルが見つかりません: {source_dir}")
        return
    
    # 現在のディレクトリ内のすべてのDLLファイルをリストアップ
    current_dlls = [f for f in os.listdir(current_dir) if f.lower().endswith('.dll')]
    
    if not current_dlls:
        print("現在のディレクトリにDLLファイルが見つかりません")
        return
    
    # 共通するDLLファイルを探して上書きコピー
    count = 0
    for dll in source_files:
        if dll in current_dlls:
            src_path = os.path.join(source_dir, dll)
            dst_path = os.path.join(current_dir, dll)
            
            try:
                shutil.copy2(src_path, dst_path)
                print(f"上書きしました: {dll}")
                count += 1
            except Exception as e:
                print(f"エラーが発生しました ({dll}): {str(e)}")
    
    print(f"\n処理完了: {count}個のDLLファイルを更新しました")

if __name__ == "__main__":
    print("同じ名前のDLLを上書きコピーするスクリプト")
    source_directory = input("DLLが格納されているディレクトリのパスを入力してください: ")
    
    if os.path.isdir(source_directory):
        overwrite_dlls_from_directory(source_directory)
    else:
        print("指定されたディレクトリが存在しません。正しいパスを入力してください。")
