import os
import shutil
from rich.console import Console
from rich.panel import Panel
from rich.progress import Progress
from rich.table import Table
from rich import box

def overwrite_matching_dlls(source_dir):
    """
    Updates DLLs in current directory by overwriting with matching DLLs from source directory.
    Only files with the same name and .dll extension will be overwritten.
    
    Args:
        source_dir (str): Path to directory containing the updated DLL files
    """
    console = Console()
    current_dir = os.getcwd()
    
    # Get DLL files from source directory
    with console.status("[bold green]Scanning source directory...") as status:
        source_dlls = [f for f in os.listdir(source_dir) 
                      if f.lower().endswith('.dll') and os.path.isfile(os.path.join(source_dir, f))]
    
    if not source_dlls:
        console.print(Panel("[red]No DLL files found in source directory", 
                           title="Error", border_style="red"))
        return
    
    # Get DLL files from current directory
    with console.status("[bold green]Scanning current directory...") as status:
        current_dlls = [f for f in os.listdir(current_dir) 
                       if f.lower().endswith('.dll') and os.path.isfile(os.path.join(current_dir, f))]
    
    if not current_dlls:
        console.print(Panel("[red]No DLL files found in current working directory", 
                           title="Error", border_style="red"))
        return
    
    # Process matching files
    updated_count = 0
    skipped_count = 0
    error_count = 0
    
    console.print(Panel("[bold green]Starting DLL update process...", 
                       title="Progress", border_style="green"))
    
    with Progress() as progress:
        task = progress.add_task("[cyan]Updating DLLs...", total=len(source_dlls))
        
        for dll in source_dlls:
            progress.update(task, advance=1, description=f"[cyan]Processing {dll[:20]}...")
            
            if dll in current_dlls:
                src_path = os.path.join(source_dir, dll)
                dst_path = os.path.join(current_dir, dll)
                
                try:
                    shutil.copy2(src_path, dst_path)
                    console.print(f"  [green]✓ Updated:[/green] [bold]{dll}[/bold]")
                    updated_count += 1
                except PermissionError:
                    console.print(f"  [red]✗ Permission denied:[/red] {dll} (try running as admin)")
                    error_count += 1
                except Exception as e:
                    console.print(f"  [red]✗ Failed to update {dll}:[/red] {str(e)}")
                    error_count += 1
            else:
                skipped_count += 1
    
    # Create summary table
    summary_table = Table(title="Update Summary", box=box.ROUNDED)
    summary_table.add_column("Metric", style="cyan")
    summary_table.add_column("Count", style="bold")
    
    summary_table.add_row("DLLs in source directory", str(len(source_dlls)))
    summary_table.add_row("DLLs in current directory", str(len(current_dlls)))
    summary_table.add_row("[green]Updated successfully", str(updated_count))
    summary_table.add_row("[yellow]Skipped (no match)", str(skipped_count))
    summary_table.add_row("[red]Errors encountered", str(error_count))
    
    console.print()
    console.print(Panel(summary_table, title="Results", border_style="blue"))

def main():
    console = Console()
    
    console.print(Panel("[bold blue]=== DLL Updater ===", 
                       subtitle="Update DLLs in current directory", 
                       box=box.DOUBLE))
    
    console.print("This tool updates DLLs in current directory by overwriting them\n"
                "with matching DLL files from another directory.\n", style="italic")
    
    source_directory = console.input("[bold]Enter path to directory containing updated DLLs:[/bold] ").strip()
    
    if not os.path.isdir(source_directory):
        console.print(Panel("[red]Error: The specified directory does not exist.", 
                           title="Error", border_style="red"))
        return
    
    overwrite_matching_dlls(source_directory)
    
    console.print("\n[bold]Operation completed.[/bold] Press Enter to exit...")
    input()

if __name__ == "__main__":
    main()
