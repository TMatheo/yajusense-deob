import os
import subprocess
from rich.console import Console
from rich.panel import Panel
from rich.table import Table
from rich.prompt import IntPrompt
from rich import print as rprint

console = Console()

def run_code2prompt(template_file):
    """Function to execute code2prompt with the specified template file and hardcoded include/exclude patterns"""
    include_pattern = "*.cs"
    
    command = [
        "code2prompt",
        ".",
        "--template",
        template_file,
        "--encoding=r50k_base",
        "--include",
        include_pattern
    ]
    
    try:
        with console.status("[bold green]Running code2prompt...", spinner="dots"):
            result = subprocess.run(command, check=True, capture_output=True, text=True)
        
        console.print(Panel.fit(
            f"[green]✓ Successfully executed code2prompt[/green]\n"
            f"Template: [bold]{template_file}[/bold]\n"
            f"Include pattern: [bold]{include_pattern}[/bold]\n",
            title="[bold]Execution Complete[/bold]",
            border_style="green"
        ))
        
    except subprocess.CalledProcessError as e:
        console.print(Panel.fit(
            f"[red]✗ Failed to execute code2prompt[/red]\n"
            f"[bold]Error:[/bold] {e}\n"
            f"[bold]Output:[/bold] {e.stdout}\n"
            f"[bold]Error Output:[/bold] {e.stderr}",
            title="[bold]Error[/bold]",
            border_style="red"
        ))
    except FileNotFoundError:
        console.print(Panel.fit(
            "[red]✗ 'code2prompt' command not found[/red]\n"
            "Please ensure it is installed and available in your system PATH.",
            title="[bold]Error[/bold]",
            border_style="red"
        ))

if __name__ == "__main__":
    hbs_dir = "hbs"
    
    # Print header
    console.print(Panel.fit(
        "[bold]code2prompt Runner[/bold]",
        subtitle="Select a template file to generate your prompt",
        border_style="blue"
    ))
    
    if not os.path.exists(hbs_dir):
        console.print(Panel.fit(
            f"[red]✗ Directory '{hbs_dir}' not found[/red]",
            title="[bold]Error[/bold]",
            border_style="red"
        ))
        exit(1)
    
    hbs_files = [f for f in os.listdir(hbs_dir) if f.endswith('.hbs')]

    if not hbs_files:
        console.print(Panel.fit(
            f"[yellow]⚠ No .hbs files found in '{hbs_dir}' directory[/yellow]",
            title="[bold]Warning[/bold]",
            border_style="yellow"
        ))
    else:
        # Create a table for the template selection
        table = Table(title="Available Template Files", show_header=True, header_style="bold magenta")
        table.add_column("No.", style="cyan", width=5)
        table.add_column("Template File", style="green")
        
        for i, filename in enumerate(hbs_files):
            table.add_row(str(i + 1), filename)
        
        console.print(table)

        while True:
            try:
                selection = IntPrompt.ask(
                    "[bold cyan]Enter the number of the template file to use[/bold cyan]",
                    choices=[str(i) for i in range(1, len(hbs_files)+1)],
                    show_choices=False
                )
                
                selected_template = os.path.join(hbs_dir, hbs_files[selection - 1])
                break
                
            except ValueError:
                console.print("[red]Invalid input. Please enter a number.[/red]")

        run_code2prompt(selected_template)
