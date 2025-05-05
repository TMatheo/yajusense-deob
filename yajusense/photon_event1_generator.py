import base64
from rich.console import Console
from rich.panel import Panel
from rich.prompt import Prompt
from rich import print

def generate_payload(original_string):
    four_bytes_null = b'\x00\x00\x00\x00'
    string_bytes = original_string.encode('utf-8')
    combined_bytes = four_bytes_null + string_bytes
    encoded_bytes = base64.b64encode(combined_bytes)
    return encoded_bytes.decode('utf-8')

def main():
    console = Console()
    print(Panel.fit("[bold cyan]Photon Event1 Payload Generator[/]", border_style="blue"))

    input_string = Prompt.ask("[bold green]Enter your string[/]")
    payload = generate_payload(input_string)

    print(Panel.fit(
        f"[bold]Input:[/] [green]{input_string}[/]\n"
        f"[bold]Payload (Base64):[/] [yellow]{payload}[/]",
        title="Result",
        border_style="green"
    ))

if __name__ == "__main__":
    main()
