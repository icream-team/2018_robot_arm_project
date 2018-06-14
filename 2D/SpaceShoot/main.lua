local composer = require "composer"

local options =
{
    effect = "fade",
    tiem = 500,
}

composer.gotoScene("lobby", options)
