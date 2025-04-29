import json
import os
from rich.console import Console
from rich.table import Table
from rich import box
from typing import List, Dict, Union
from pathlib import Path

def load_data_from_file(filename: str) -> Dict[str, List[Dict]]:
    """Load JSON data from file with validation"""
    if not os.path.exists(filename):
        raise FileNotFoundError(f"File not found: {filename}")
    if not filename.lower().endswith('.json'):
        raise ValueError("File must be a JSON file (.json extension)")
    
    try:
        with open(filename, 'r', encoding='utf-8') as f:
            data = json.load(f)
            if not isinstance(data, dict) or 'ScriptMethod' not in data:
                raise ValueError("Invalid JSON structure - expected Unity metadata format")
            return data
    except json.JSONDecodeError:
        raise ValueError("Invalid JSON format in file")

def find_items_by_address(data: Dict[str, List[Dict]], hex_address: str) -> Dict[str, Union[Dict, None]]:
    """Search for items by address in both ScriptMethod and ScriptString"""
    try:
        decimal_address = int(hex_address, 16)
    except ValueError:
        return {"ScriptMethod": None, "ScriptString": None}
    
    results = {
        "ScriptMethod": None,
        "ScriptString": None
    }
    
    for item in data.get("ScriptMethod", []):
        if item["Address"] == decimal_address:
            results["ScriptMethod"] = item
            break
    
    for item in data.get("ScriptString", []):
        if item["Address"] == decimal_address:
            results["ScriptString"] = item
            break
    
    return results

def display_results(results: Dict[str, Union[Dict, None]]):
    """Display search results with rich formatting"""
    console = Console()
    
    if results["ScriptMethod"]:
        func = results["ScriptMethod"]
        table = Table(
            title=f"ScriptMethod (0x{func['Address']:X})",
            box=box.ROUNDED,
            header_style="bold magenta"
        )
        table.add_column("Field", style="cyan", width=20)
        table.add_column("Value", style="green")
        table.add_row("Address", f"0x{func['Address']:X}")
        table.add_row("Name", func['Name'])
        table.add_row("Signature", func['Signature'])
        table.add_row("Type Signature", func['TypeSignature'])
        console.print(table)
    
    if results["ScriptString"]:
        string_item = results["ScriptString"]
        table = Table(
            title=f"ScriptString (0x{string_item['Address']:X})",
            box=box.ROUNDED,
            header_style="bold blue"
        )
        table.add_column("Field", style="cyan", width=20)
        table.add_column("Value", style="green")
        table.add_row("Address", f"0x{string_item['Address']:X}")
        table.add_row("String Value", string_item['Value'])
        console.print(table)
    
    if not any(results.values()):
        console.print("[red]No items found at this address[/red]", style="bold")

def get_file_path(console: Console) -> str:
    """Prompt user for JSON file path with validation"""
    while True:
        path = console.input("\nEnter path to JSON metadata file: ").strip('"\' ')
        
        if not path:
            console.print("[yellow]Please enter a file path[/yellow]")
            continue
            
        path = os.path.expanduser(path)
        
        if path.lower() in ('q', 'quit', 'exit'):
            return None
            
        if not os.path.exists(path):
            console.print(f"[red]Error: File not found - {path}[/red]")
            continue
            
        if not path.lower().endswith('.json'):
            console.print("[red]Error: File must be a JSON file (.json extension)[/red]")
            continue
            
        return path

def main():
    console = Console()
    console.print("[bold]Unity Metadata Explorer[/bold]", style="blue")
    console.print("This tool searches ScriptMethod and ScriptString tables in Unity metadata", style="italic")
    console.print("Enter 'q' at any time to quit", style="italic")
    
    # Get JSON file path
    json_path = get_file_path(console)
    if not json_path:
        return
    
    # Load data
    try:
        data = load_data_from_file(json_path)
        console.print(f"\n[green]Successfully loaded:[/green] {json_path}")
    except Exception as e:
        console.print(f"[red]Error loading file:[/red] {str(e)}", style="bold")
        return
    
    # Interactive search
    while True:
        console.print("\n[bold]Search Options:[/bold]")
        console.print("1. Search by address")
        console.print("2. Change JSON file")
        console.print("3. Exit")
        
        choice = console.input("\nEnter your choice (1-3): ").strip()
        
        if choice == '1':
            while True:
                hex_input = console.input("\nEnter hexadecimal address (or 'b' to go back): ").strip()
                
                if hex_input.lower() in ('b', 'back'):
                    break
                if hex_input.lower() in ('q', 'quit', 'exit'):
                    return
                if not hex_input:
                    continue
                    
                if not hex_input.startswith('0x'):
                    hex_input = '0x' + hex_input
                    
                results = find_items_by_address(data, hex_input)
                display_results(results)
                
        elif choice == '2':
            new_path = get_file_path(console)
            if new_path:
                try:
                    data = load_data_from_file(new_path)
                    json_path = new_path
                    console.print(f"\n[green]Successfully loaded:[/green] {json_path}")
                except Exception as e:
                    console.print(f"[red]Error loading file:[/red] {str(e)}", style="bold")
        
        elif choice in ('3', 'q', 'quit', 'exit'):
            return
        
        else:
            console.print("[red]Invalid choice. Please enter 1, 2, or 3.[/red]")

if __name__ == "__main__":
    main()
