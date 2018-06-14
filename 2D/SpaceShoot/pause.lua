local composer = require "composer"
local bf = require "basic_features"

local scene = composer.newScene()


-- -----------------------------------------------------------------------------------
-- Code outside of the scene event functions below will only be executed ONCE unless
-- the scene is removed entirely (not recycled) via "composer.removeScene()"
-- -----------------------------------------------------------------------------------


local score

function return_game( )

    local sceneOption =
    {
        effect = "fade",
        time = 500,
    }
    composer.hideOverlay( "game", sceneOption )
end


-- -----------------------------------------------------------------------------------
-- Scene event functions
-- -----------------------------------------------------------------------------------


-- create()
function scene:create( e )

    local sceneGroup = self.view
    local params = e.params

    score = params.gamescore

end


-- show()
function scene:show( e )

    local sceneGroup = self.view
    local phase = e.phase

    if phase == "will" then
        -- Code here runs when the scene is still off screen ( but is about to come on screen )

    elseif phase == "did" then
        -- Code here runs when the scene is entirely on screen

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
