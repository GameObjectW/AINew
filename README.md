# AINew
初版简单AI脚本，应用组合的设计模式，来是的AI的逻辑判断与最终的执行节点相分离，在后期的修改中，只需要修改相应的Action脚本或添加新的Action脚本即可。
- 2018.1.28<br>
1. 修改AI脚本中的部分函数，新增基于技能`CD`和`Weight`的AI的技能释放管理系统<br>
2. 新增`JZSoliderAI`和`LineSoliderAI`两种AI实现模板（前者为仅有普通攻击的AI实现，后者为带有技能的AI实现）
3. 技能的释放根据设定的CD和Weight，通过优先级队列将队列中可释放的并且权重最高的技能取出并触发，并将技能放入到`CdCalList`中进行技能冷却，冷而却完成后添加到优先级队列中。
4. 为`AINew`、`LineSoliderAI`、`SkillCDAndWeight_New`脚本添加详细注释
