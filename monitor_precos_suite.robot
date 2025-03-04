*** Settings ***
Library           SeleniumLibrary
Resource          monitor_precos.robot

*** Test Cases ***
Buscar preços do produto e salvar no banco
    Abrir o browser e navegar até a página do shopping Google
    Digitar e buscar o produto "Playstation 5"
    Selecionar a opção de Itens novos
    Armazenar em uma lista os items encontrados do produto "Playstation 5" e enviar por Json para a API