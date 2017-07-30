(function() {
    $(function() {
        $("#iteration-collapse-all").on("click", onClickIterationCollapseAll);
        $("#iteration-expand-all").on("click", onClickIterationExpandAll);
        $(".area-path-item").on("click", onClickAreaPathItem);

        var $iterationSearchText = $("#iteration-search-text");

        $("#iteration-search-text-clear").on("click",
            function() {
                $iterationSearchText.val("");

                onClickIterationExpandAll();
            });

        var debouncedOnIterationSearchTextChange = debounce(onIterationSearchTextChange, 750);

        $iterationSearchText.on("change input", debouncedOnIterationSearchTextChange);
    });

    function onClickIterationCollapseAll() {
        $(".iteration-work-iteration-items .iteration-parent-collapse").collapse("hide");
    }

    function onClickIterationExpandAll() {
        $(".iteration-work-iteration-items .iteration-parent-collapse").collapse("show");
    }

    function onClickAreaPathItem() {
        var areaPath = $(this).data("area-path"),
            $allElements = $(".iteration-parent-collapse"),
            $showElements = $allElements.filter("[data-area-path='" + areaPath + "']"),
            $hideElements = $allElements.not($showElements);

        $hideElements.collapse("hide");
        $showElements.collapse("show");
    }

    function onIterationSearchTextChange() {
        var text = $(this).val(),
            textRegExp = new RegExp(text, "i"),
            $workItems = $(".iteration-work-iteration-items .iteration-parent-collapse"),
            workItemsLength = $workItems.length;

        for (var i = 0; i < workItemsLength; i++) {
            var $item = $workItems.eq(i),
                $parent = $item.parent(),
                storyHeader = $("header span", $parent).first().text() || "",
                storyDescription = $(".work-iteration-item-description", $item).first().text() || "",
                hasMatch = (storyHeader.search(textRegExp) >= 0) ||
                    (storyDescription.search(textRegExp) >= 0);

            if (!hasMatch) {
                var $children = $(".work-iteration-item-children .work-iteration-item-child", $item),
                    childrenLength = $children.length;

                for (var j = 0; j < childrenLength; j++) {
                    var $child = $children.eq(j),
                        childHeader = $("header span", $child).text() || "",
                        hasChildMatch = (childHeader.search(textRegExp) >= 0);

                    if (hasChildMatch) {
                        hasMatch = true;

                        $(".work-iteration-item-children .work-iteration-item-children-content").collapse("show");
                        break;
                    }
                }
            }

            var collapseAction = hasMatch ? "show" : "hide";

            $item.collapse(collapseAction);
        }
    }

    function debounce(fn, delay) {
        var timer = null;
        return function() {
            var context = this, args = arguments;
            clearTimeout(timer);
            timer = setTimeout(function() {
                    fn.apply(context, args);
                },
                delay);
        };
    }
})();