local composer = require "composer"
local bf = require "basic_features"

local scene = composer.newScene()

-- -----------------------------------------------------------------------------------
-- Code outside of the scene event functions below will only be executed ONCE unless
-- the scene is removed entirely (not recycled) via "composer.removeScene()"
-- -----------------------------------------------------------------------------------

local phase = 0

function game_scene( )
    local sceneOption =
    {
        effect = "fade",
        time = 500
    }
    composer.removeScene( "lobby", sceneOption )
    composer.gotoScene( "game", sceneOption )
end


function key_event( e )

    local key_name = e.keyName

    if e.phase == "down" then
        if key_name == "enter" or key_name == "space" then
            phase = 1
            Runtime:removeEventListener( "key", key_event )
            game_scene()
        end
    end

end

local press_button_to_start, title

function create_display( )

    function start_fade( e )
        if phase == 0 then
            if press_button_to_start.alpha == 0 then
                transition.to( press_button_to_start, { time = 750, alpha = 1 })
            else
                transition.to( press_button_to_start, { time = 750, alpha = 0 })
            end
        else
            if e.source then
                timer.cancel( e.source )
            end
        end
    end


    title = display.newText( "space shoot", bf.W*0.5, bf.H*0.3, bf.font, bf.font_size_b, "center")
    title:setFillColor( bf.hexToPercent( "ffffff" ) )

    press_button_to_start = display.newText( "Press Button to Start", bf.W*0.5, bf.H*0.75, bf.font, bf.font_size, "center")
    timer.performWithDelay( 800, start_fade, -1 )

end

function delete_display()
    press_button_to_start:removeSelf()
    title:removeSelf()
end


-- -----------------------------------------------------------------------------------
-- Scene event functions
-- -----------------------------------------------------------------------------------


-- create()
function scene:create( e )

    local sceneGroup = self.view

    create_display()
end


-- show()
function scene:show( e )

    local sceneGroup = self.view
    local phase = e.phase

    if phase == "will" then
        -- Code here runs when the scene is still off screen ( but is about to come on screen )

    elseif phase == "did" then
        -- Code here runs when the scene is entirely on screen
        Runtime:addEventListener( "key", key_event )

    end
end


-- hide()
function scene:hide( e )

    local sceneGroup = self.view
    local phase = e.phase

    if phase == "will" then
        -- Code here runs when the scene is on screen (but is about to go off screen)

    elseif phase == "did" then
        -- Code here runs immediately after the scene goes entirely off screen

    end
end


-- destroy()
function scene:destroy( e )

    local sceneGroup = self.view
    -- Code here runs prior to the removal of scene's view

    delete_display()

end


-- -----------------------------------------------------------------------------------
-- Scene event function listeners
-- -----------------------------------------------------------------------------------
scene:addEventListener( "create", scene )
scene:addEventListener( "show", scene )
scene:addEventListener( "hide", scene )
scene:addEventListener( "destroy", scene )
-- -----------------------------------------------------------------------------------

return scene
