1、2.5d视角的实现参考：https://www.bilibili.com/video/BV1DT4y1A7DJ

qe可以转视角

但是把混合树改成了rotate实现，人物不挂facingcam



2、场景的破环instaciate了animation做成的prefab，因为动画的替换不知道为啥不好使



3、人物的三段攻击现在dino和fsm中，其他写在playemovement中，这么变扭是因为硬用fsm，但fsm又带来手感上的问题，当然大概率是因为我写的fsm不行，但这玩意儿哪里能改我是想不到



4、监听者模式instanciate加了个get，因为在hierarchy里不好找



5、最后时间来不及了（都快被我拖成gamejam了）好多bug，技能全简化成攻击了，特别是ui也撑小了不知道为啥，但太累了不想改了，做的人有点自闭，而且做完之后还不能打游戏放松呜呜

但是，长了个教训以及这次真的写了好多脚本，还是有点牛牛牛在的



​                                                                                                                                          ——godzillaLord







