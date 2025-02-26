*** Settings ***
Library           SeleniumLibrary
Library           RequestsLibrary
Library           JSONLibrary
Library           String
Library           Collections

*** Variables ***
${URL}           https://shopping.google.com.br
${BROWSER}       headlesschrome
${API_URL}       http://localhost:5231/api/produtos
${COUNT}         1

*** Test Cases ***
Buscar preços do produto "Metal Zone" e salvar no banco
    Open Browser    ${URL}    ${BROWSER}
    Sleep    5s  
    
    Wait Until Element Is Visible    locator=//*[@placeholder='O que você está procurando?']    timeout=20s
    Input Text    locator=//*[@placeholder='O que você está procurando?']    text=Metal Zone
    Wait Until Element Is Visible    locator=css:*[action='https://www.google.com.br/search?tbm=shop'] ul li:nth-child(1)    timeout=10s
    Click Element    locator=css:*[action='https://www.google.com.br/search?tbm=shop'] ul li:nth-child(1)
    Sleep    10s

    ${products}    Create List
    ${items}    Get WebElements    locator=css:.sh-dgr__content

        FOR    ${item}    IN    @{items}
            ${validarCondicao}    Run Keyword And Return Status    Element Should Contain    locator=(//*[@class='sh-dgr__content'])[${COUNT}]//*[@class='tAxDx']    expected=Metal Zone
            IF    ${validarCondicao} == $True
                ${name}    Get Text    locator=xpath:(//*[@class='sh-dgr__content'])[${COUNT}]//*[@class='tAxDx']
                ${price}    Get Text    locator=xpath:(//*[@class='sh-dgr__content'])[${COUNT}]//*[@class='a8Pemb OFFNJ']
                ${store}    Get Text    locator=xpath:(//*[@class='sh-dgr__content'])[${COUNT}]//*[@class='aULzUe IuHnof']
                ${url}      Get Element Attribute    locator=xpath:(//*[@class='sh-dgr__content'])[${COUNT}]//*[@class='mnIHsc']/a[1]    attribute=href

                ${product} =    Create Dictionary
                ...    NomeProduto=${name}
                ...    PrecoProduto=${price}
                ...    Url=${url}
                ...    Loja=${store}
                Append To List    ${products}    ${product}
                ${COUNT}    Evaluate    ${COUNT} + 1
            END
        END

    Close Browser

    ${response} =    POST    ${API_URL}    json=${products}
    Log    ${response.json()}
