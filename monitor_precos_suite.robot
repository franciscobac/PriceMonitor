*** Settings ***
Library           SeleniumLibrary
Resource          monitor_precos.robot

*** Test Cases ***
Buscar preços do produto e salvar no banco
    Abrir o browser e navegar até a página do Buscape
    Digitar e buscar o produto "Playstation 5"
    Armazenar em uma lista os items encontrados do produto "Playstation 5" e enviar por Json para a API
