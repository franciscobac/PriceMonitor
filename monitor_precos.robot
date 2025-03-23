*** Settings ***
Library           SeleniumLibrary
Library           RequestsLibrary
Library           JSONLibrary
Library           String
Library           Collections
Library           DateTime

*** Variables ***
${URL}           http://buscape.com.br
${BROWSER}       headlesschrome
${API_URL}       http://localhost:5231/api/produtos
${COUNT}         1

*** Keywords ***
Abrir o browser e navegar até a página do Buscape
    ${chrome_options}=    Evaluate    sys.modules['selenium.webdriver'].ChromeOptions()    sys, selenium.webdriver
    Call Method    ${chrome_options}    add_argument    --headless
    Call Method    ${chrome_options}    add_argument    --disable-gpu
    Call Method    ${chrome_options}    add_argument    --no-sandbox
    Call Method    ${chrome_options}    add_argument    --disable-dev-shm-usage
    Call Method    ${chrome_options}    add_argument    --user-agent\=Your Own user agent data
    Open Browser    ${URL}    ${BROWSER}    options=${chrome_options}
    Sleep    5s

# Check pop-ups or modals
    ${modal_exists}=    Run Keyword And Return Status    Element Should Be Visible    xpath://*[contains(text(), 'Concordar')]
    IF    ${modal_exists} == $True
        Click Element    xpath://*[contains(text(), 'Concordar')]
    END
    Sleep    2s

Digitar e buscar o produto "${Produto}"
    Wait Until Element Is Visible    locator=${inputSearch}    timeout=20s
    Input Text    locator=${inputSearch}    text=${Produto}
    Wait Until Element Is Visible    locator=${buttonSearch}    timeout=10s
    Click Element    locator=${buttonSearch}
    Sleep    10s

    # Check pop-ups or modals (if exist)
    ${popup_exists}=    Run Keyword And Return Status    Element Should Be Visible    xpath://*[@class='ModalCampaign_CloseButton__LDTLX']//button
    IF    ${popup_exists} == $True
        Click Element    xpath://*[@class='ModalCampaign_CloseButton__LDTLX']//button
    END
    Sleep    2s

Armazenar em uma lista os items encontrados do produto "${Produto}" e enviar por Json para a API
    ${products}    Create List
    Sleep    5s
    
    ${items}    Get WebElements    locator=${cardProductLowestPrice}
    Sleep    5s

        FOR    ${item}    IN    @{items}
            Sleep    500ms
            #Scroll Element Into View    locator=${cardProductLowestPrice}[${COUNT}]

	    # Build XPath dynamically for the currently item
            ${current_card}=    Set Variable    (${cardProductLowestPrice})[${COUNT}]
            Scroll Element Into View    locator=${current_card}
            Capture Page Screenshot    screenshot_${COUNT}.png  # cat the screenshot            
	    
	    # Build XPath dynamically for the currently item
            ${current_card}=    Set Variable    (${cardProductName})[${COUNT}]
            
	    ${validarCondicao}    Run Keyword And Return Status    Element Should Contain    locator=${current_card}    expected=${Produto}    ignore_case=true
            IF    ${validarCondicao} == $True
                ${name}    Get Text    locator=(//*[@class='Hits_ProductCard__Bonl_']//*[contains(text(),'Menor preço')]/ancestor::*[@class='ProductCard_ProductCard_Inner__gapsh']//*[@data-testid='product-card::name'])[${COUNT}]
                ${price}    Get Text    locator=(//*[@class='Hits_ProductCard__Bonl_']//*[contains(text(),'Menor preço')]/../..//*[@data-testid='product-card::price'])[${COUNT}]
                ${storeSubstring}    Get Text    locator=(//*[@class='Hits_ProductCard__Bonl_']//*[contains(text(),'Menor preço')])[${COUNT}]
                ${store}    Get Substring    ${storeSubstring}    15
                ${url}      Get Element Attribute    locator=(//*[@class='Hits_ProductCard__Bonl_']//*[contains(text(),'Menor preço')]/ancestor::*[@class='ProductCard_ProductCard_Inner__gapsh'])[${COUNT}]    attribute=href
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
