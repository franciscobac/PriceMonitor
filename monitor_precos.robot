*** Settings ***
Library           SeleniumLibrary
Library           RequestsLibrary
Library           JSONLibrary
Library           String
Library           Collections
Library           DateTime

*** Variables ***
${URL}           https://shopping.google.com.br
${BROWSER}       headlesschrome
${API_URL}       http://localhost:5231/api/produtos
${COUNT}         1

*** Keywords ***
Abrir o browser e navegar até a página do shopping Google
    Open Browser    ${URL}    ${BROWSER}
    Sleep    5s

Digitar e buscar o produto "${Produto}"
    Wait Until Element Is Visible    locator=//*[@placeholder='O que você está procurando?']    timeout=20s
    Input Text    locator=//*[@placeholder='O que você está procurando?']    text=${Produto}
    Wait Until Element Is Visible    locator=css:*[action='https://www.google.com.br/search?tbm=shop'] ul li:nth-child(1)    timeout=10s
    Click Element    locator=css:*[action='https://www.google.com.br/search?tbm=shop'] ul li:nth-child(1)
    Sleep    10s

Selecionar a opção de Itens novos
    Scroll Element Into View    locator=xpath://*[contains(text(), 'Itens novos')]
    Wait Until Element Is Visible    locator=xpath://*[contains(text(), 'Itens novos')]    timeout=10s
    Click Element    locator=xpath://*[contains(text(), 'Itens novos')]
    Sleep    10s

Armazenar em uma lista os items encontrados do produto "${Produto}" e enviar por Json para a API
    ${products}    Create List
    Sleep    5s
    
    ${items}    Get WebElements    locator=css:*[class='sh-dgr__content']
    Sleep    5s

        FOR    ${item}    IN    @{items}
            Sleep    500ms
            Scroll Element Into View    locator=xpath:(//*[@class='sh-dgr__content'])[${COUNT}]//*[@class='tAxDx']
            ${validarCondicao}    Run Keyword And Return Status    Element Should Contain    locator=(//*[@class='sh-dgr__content'])[${COUNT}]//*[@class='tAxDx']    expected=${Produto}    ignore_case=true
            IF    ${validarCondicao} == $True
                ${name}    Get Text    locator=xpath:(//*[@class='sh-dgr__content'])[${COUNT}]//*[@class='tAxDx']
                ${price}    Get Text    locator=xpath:(//*[@class='sh-dgr__content'])[${COUNT}]//*[@class='a8Pemb OFFNJ']
                ${store}    Get Text    locator=xpath:(//*[@class='sh-dgr__content'])[${COUNT}]//*[@class='aULzUe IuHnof']
                ${url}      Get Element Attribute    locator=xpath:(//*[@class='sh-dgr__content'])[${COUNT}]//*[@class='mnIHsc']/a[1]    attribute=href
                ${data_coleta}    Get Current Date
                ${data_formatada}    Convert Date    ${data_coleta}    result_format=%Y-%m-%dT%H:%M:%S

                ${product} =    Create Dictionary
                ...    NomeProduto=${name}
                ...    PrecoProduto=${price}
                ...    Url=${url}
                ...    Loja=${store}
                ...    DataColeta=${data_formatada}
                
                Append To List    ${products}    ${product}
            END

            ${COUNT}    Evaluate    ${COUNT} + 1
        END

    Capture Page Screenshot
    Close Browser

    ${response} =    POST    ${API_URL}    json=${products}
    Log    ${response.json()}